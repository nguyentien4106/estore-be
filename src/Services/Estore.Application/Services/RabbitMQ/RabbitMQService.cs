using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using EStore.Application.Models.Files;
using System.Text.Json;
using EStore.Application.Services.Telegram;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.Application.Services.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName = "chunk";
        private const string _exchangeName = "file-exchange";

        private IConnection? _connection;
        private readonly IChannel _producerChannel;
        private readonly ITelegramService _telegramService;
        
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentDictionary<string, ConcurrentBag<(ChunkMessage Message, ulong DeliveryTag)>> _messageStore = new();

        public RabbitMQService(
            RabbitMQConfiguration rabbitMQOptions,
            ITelegramService telegramService,
            IServiceScopeFactory factory)
        {
            _telegramService = telegramService;
            _serviceScopeFactory = factory;
            _telegramService = telegramService;
            _factory = new ConnectionFactory()
            {
                HostName = rabbitMQOptions.HostName,
                UserName = rabbitMQOptions.UserName,
                Password = rabbitMQOptions.Password,
                VirtualHost = rabbitMQOptions.VirtualHost
            };
            _connection = _factory.CreateConnectionAsync().GetAwaiter().GetResult();

            _producerChannel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _producerChannel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true).GetAwaiter();
            _producerChannel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null).GetAwaiter();
            _producerChannel.QueueBindAsync(_queueName, _exchangeName, "chunk").GetAwaiter();

        }

        public async Task<bool> ProducerAsync(string message)
        {
            if (_connection is null || !_connection.IsOpen || _producerChannel is null)
            {
                Console.WriteLine("RabbitMQ channel is not available for producer.");
                return false;
            }
            var channel = _producerChannel;
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                var properties = new BasicProperties() { Persistent = true };

                await channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: _queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
                Console.WriteLine($"Sent message to {_queueName}: {message}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message via ProducerAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<AppResponse<string>> ConsumerAsync(string consumerTag)
        {
            try
            {
                var channel = await _connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                await channel.QueueBindAsync(_queueName, _exchangeName, "chunk");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = JsonSerializer.Deserialize<ChunkMessage>(Encoding.UTF8.GetString(body));
                        Console.WriteLine($"Received message for chunk {message.ChunkIndex} of file {message.FileId}");

                        // Store message and delivery tag
                        var messagesForFile = _messageStore.GetOrAdd(message.FileId, _ => new ConcurrentBag<(ChunkMessage, ulong)>());
                        messagesForFile.Add((message, ea.DeliveryTag));

                        // Check if all chunks are received
                        if (messagesForFile.Count == message.TotalChunks)
                        {
                            var id = message.Id;

                            // Verify all chunks exist in S3
                            if (AreAllChunksAvailable(message))
                            {
                                var mergedFileStream = await MergeChunksAsync(message);
                                await UploadToTelegramAsync(mergedFileStream, message);
                                await UpdateDatabaseAsync(id, message.UserId);
                                // Acknowledge all messages for this file
                                await CleanUpAsync(channel, message, messagesForFile);

                            }
                            else
                            {
                                Console.WriteLine($"Not all chunks available in S3 for file {message.FileId}. Waiting...");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Waiting for {message.TotalChunks - messagesForFile.Count} more chunks for file {message.FileId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                var result = await channel.BasicConsumeAsync(_queueName, autoAck: false, consumerTag, consumer);
                Console.WriteLine($"[{consumerTag}] Consumer started on queue '{_queueName}'. Waiting for messages.");
                return AppResponse<string>.Success(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting consumer [{consumerTag}]: {ex.Message}");
                return AppResponse<string>.Error(ex.Message);
            }
        }

        private bool AreAllChunksAvailable(ChunkMessage message)
        {
            // get number of chunks in directory
            var directoryPath = Path.Combine(AppContext.BaseDirectory, "temps", message.UserId, message.FileId);
            var chunks = Directory.GetFiles(directoryPath);
            return chunks.Length == message.TotalChunks;
        }

        private async Task<Stream> MergeChunksAsync(ChunkMessage message)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "temps", message.UserId, message.FileId);
                var chunks = Directory.GetFiles(filePath);

                // store to local disk
                var mergedFilePath = Path.Combine(AppContext.BaseDirectory, "results", message.UserId, message.FileId, message.FileName);

                // Ensure the directory exists before writing the file
                var mergedFilePathDirectory = Path.GetDirectoryName(mergedFilePath);
                if (mergedFilePathDirectory != null && !Directory.Exists(mergedFilePathDirectory))
                {
                    Directory.CreateDirectory(mergedFilePathDirectory); // This creates all directories in the path if they don't exist
                }
                
                var mergedFileStream = File.Create(mergedFilePath);
                foreach (var chunk in chunks)
                {
                    using (var chunkFileStream = File.OpenRead(chunk))
                    {
                        await chunkFileStream.CopyToAsync(mergedFileStream);
                    }
                }
                Console.WriteLine($"Merged file {message.FileId} into {mergedFilePath}");   
                return mergedFileStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MergeChunksAsync: {ex.Message}");
                throw;
            }
        }

        private async Task CleanUpAsync(IChannel channel, ChunkMessage message, ConcurrentBag<(ChunkMessage, ulong)> messagesForFile)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "temps", message.UserId, message.FileId);
            var resultPath = Path.Combine(AppContext.BaseDirectory, "results", message.UserId, message.FileId);
            Directory.Delete(filePath, true);
            Directory.Delete(resultPath, true);

            foreach (var (_, deliveryTag) in messagesForFile)
            {
                await channel.BasicAckAsync(deliveryTag: deliveryTag, multiple: false);
            }
            _messageStore.TryRemove(message.FileId, out _);
        }

        private async Task UploadToTelegramAsync(Stream mergedFileStream, ChunkMessage message)
        {
            try
            {
                mergedFileStream.Position = 0;
                var args = new UploadFileHandlerArgs
                {
                    FileStream = mergedFileStream,
                    FileName = message.FileName,
                    ContentType = message.ContentType,
                    ContentLength = message.ContentLength,
                };
                var result = await _telegramService.UploadFileToStrorageAsync(args, message.UserId);

                if (result.Succeed)
                {
                    Console.WriteLine($"Uploaded file {message.FileId} to Telegram");
                }
                else
                {
                    Console.WriteLine($"Error uploading file {message.FileId} to Telegram: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UploadToTelegramAsync: {ex.Message}");
                throw;
            }
            finally
            {
                mergedFileStream.Close();
            }
        }

        private async Task UpdateDatabaseAsync(Guid id, string userId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IEStoreDbContext>();
                var file = await context.TeleFileEntities.FindAsync(id);
                Console.WriteLine($"Updating file {id} to {FileStatus.Uploaded}");
                if (file != null)
                {
                    file.FileStatus = FileStatus.Uploaded;
                    await context.CommitAsync();
                }
            }
        }
        
        public void Dispose()
        {
            try
            {
                _connection?.CloseAsync().GetAwaiter().GetResult();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during RabbitMQService Dispose: {ex.Message}");
            }
            GC.SuppressFinalize(this);
        }
    }
} 