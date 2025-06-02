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
using Microsoft.Extensions.Logging;
using EStore.Application.Helpers;
using System.Linq;
using EStore.Application.Constants;

namespace EStore.Application.Services.RabbitMQ
{
    public class PushFileMessage
    {
        public string FilePath { get; set; }
        public Guid FileId { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
    }

    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private readonly IChannel _producerChannel;
        private IChannel _mergeFileConsumerChannel;
        private IChannel _pushFileConsumerChannel;
        private readonly ITelegramService _telegramService;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ILogger<RabbitMQService> _logger;
        private readonly ConcurrentDictionary<string, ConcurrentBag<(ChunkMessage Message, ulong DeliveryTag)>> _messageStore = new();

        public RabbitMQService(
            RabbitMQConfiguration rabbitMQOptions,
            ITelegramService telegramService,
            IServiceScopeFactory factory,
            ILogger<RabbitMQService> logger)
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
            _producerChannel.ExchangeDeclareAsync(QueueConstants.ExchangeName, ExchangeType.Direct, durable: true).GetAwaiter();
            _producerChannel.QueueDeclareAsync(QueueConstants.MergeFileQueue, durable: true, exclusive: false, autoDelete: false, arguments: null).GetAwaiter();
            _producerChannel.QueueBindAsync(QueueConstants.MergeFileQueue, QueueConstants.ExchangeName, "chunk").GetAwaiter();

            _logger = logger;
        }

        public async Task<bool> ProducerAsync(string queueName, string message)
        {
            if (_connection is null || !_connection.IsOpen || _producerChannel is null)
            {
                _logger.LogError("RabbitMQ channel is not available for producer.");
                return false;
            }

            var channel = _producerChannel;
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                var properties = new BasicProperties() { Persistent = true };

                await channel.BasicPublishAsync(
                    exchange: QueueConstants.ExchangeName,
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message via ProducerAsync: {ex.Message}");
                return false;
            }
        }

        public async Task MergeFileConsumerAsync(string consumerTag)
        {
            try
            {
                _mergeFileConsumerChannel = await _connection.CreateChannelAsync();
                await _mergeFileConsumerChannel.QueueDeclareAsync(QueueConstants.MergeFileQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                await _mergeFileConsumerChannel.QueueBindAsync(QueueConstants.MergeFileQueue, QueueConstants.ExchangeName, QueueConstants.MergeFileQueue);

                var consumer = new AsyncEventingBasicConsumer(_mergeFileConsumerChannel);
                consumer.ReceivedAsync += HandleReceivedMessageMergeFileAsync;

                await _mergeFileConsumerChannel.BasicConsumeAsync(QueueConstants.MergeFileQueue, autoAck: false, consumerTag, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting consumer [{consumerTag}]: {ex.Message}");
                throw;
            }
        }

        private async Task HandleReceivedMessageMergeFileAsync(object model, BasicDeliverEventArgs ea)
        {
            Stream? mergedFileStream = null;
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<ChunkMessage>(Encoding.UTF8.GetString(body));

                var messagesForFile = _messageStore.GetOrAdd(message.FileId, _ => []);
                messagesForFile.Add((message, ea.DeliveryTag));

                if (messagesForFile.Count == message.TotalChunks)
                {
                    var id = message.Id != Guid.Empty ? message.Id : messagesForFile.FirstOrDefault(item => item.Message.Id != Guid.Empty).Message.Id;

                    var mergedFilePath = await MergeChunksAsync(message);
                    if(string.IsNullOrEmpty(mergedFilePath))
                    {
                        _logger.LogError($"Error merging chunks for file {message.FileId}, {mergedFilePath}");
                        return;
                    }
                    var pushFileMessage = new PushFileMessage
                    {
                        FilePath = mergedFilePath,
                        FileId = id,
                        UserId = message.UserId,
                        FileName = message.FileName,
                        ContentType = message.ContentType
                    };
                    _logger.LogInformation($"Pushing {QueueConstants.PushFileQueue} with message {JsonSerializer.Serialize(pushFileMessage)}");
                    await ProducerAsync(QueueConstants.PushFileQueue, JsonSerializer.Serialize(pushFileMessage));
                    await CleanUpMergeFileAsync(_mergeFileConsumerChannel, message, messagesForFile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
                await _mergeFileConsumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
            finally
            {
                mergedFileStream?.Close();
            }
        }

        private async Task<string> MergeChunksAsync(ChunkMessage message)
        {
            try
            {
                var filePath = FileHelper.GetTempFilePathPart(message.UserId, message.FileId);
                var chunks = Directory.GetFiles(filePath);

                // order by chunk index
                chunks = [.. chunks.OrderBy(chunk => int.Parse(chunk.Split(Path.DirectorySeparatorChar).Last()))];
                var mergedFilePath = Path.Combine(AppContext.BaseDirectory, "results", message.UserId, message.FileId, message.FileName);

                var mergedFilePathDirectory = Path.GetDirectoryName(mergedFilePath);
                if (mergedFilePathDirectory != null && !Directory.Exists(mergedFilePathDirectory))
                {
                    Directory.CreateDirectory(mergedFilePathDirectory);
                }

                using var mergedFileStream = File.Create(mergedFilePath);
                foreach (var chunk in chunks)
                {
                    using var chunkFileStream = File.OpenRead(chunk);
                    await chunkFileStream.CopyToAsync(mergedFileStream);
                }

                return mergedFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MergeChunksAsync: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task CleanUpMergeFileAsync(IChannel channel, ChunkMessage message, ConcurrentBag<(ChunkMessage, ulong)> messagesForFile)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "temps", message.UserId, message.FileId);
            Directory.Delete(filePath, true);

            foreach (var (_, deliveryTag) in messagesForFile)
            {
                await channel.BasicAckAsync(deliveryTag: deliveryTag, multiple: false);
            }
            _messageStore.TryRemove(message.FileId, out _);
        }



        public async Task PushFileConsumerAsync(string consumerTag)
        {
            try
            {
                _pushFileConsumerChannel = await _connection.CreateChannelAsync();
                await _pushFileConsumerChannel.QueueDeclareAsync(QueueConstants.PushFileQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                await _pushFileConsumerChannel.QueueBindAsync(QueueConstants.PushFileQueue, QueueConstants.ExchangeName, QueueConstants.PushFileQueue);   

                var consumer = new AsyncEventingBasicConsumer(_pushFileConsumerChannel);
                consumer.ReceivedAsync += HandleReceivedMessagePushFileAsync;

                await _pushFileConsumerChannel.BasicConsumeAsync(QueueConstants.PushFileQueue, autoAck: false, consumerTag, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting consumer [{consumerTag}]: {ex.Message}");
                throw;
            }
        }

        private async Task HandleReceivedMessagePushFileAsync(object model, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<PushFileMessage>(Encoding.UTF8.GetString(body));
                Console.WriteLine($"message received: {JsonSerializer.Serialize(message)}");
                if (message is null)
                {
                    _logger.LogError($"Invalid message: {Encoding.UTF8.GetString(body)}");
                    return;
                }
                await _pushFileConsumerChannel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                var teleFileEntity = await UploadToTelegramAsync(message.FilePath, message);
                if (teleFileEntity.Succeed)
                {
                    await UpdateDatabaseAsync(message.FileId, teleFileEntity.Data);
                    Console.WriteLine($"message.FilePath: {message.FilePath}");
                    CleanUpPushFile(message.FilePath);
                }
                else
                {
                    _logger.LogError($"Error uploading file {message.FileId} to Telegram: {teleFileEntity.Message}");
                    await _pushFileConsumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message in push file consumer: {ex.Message}");
                await _pushFileConsumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        }

        private async Task<AppResponse<TeleFileEntity>> UploadToTelegramAsync(string mergedFilePath, PushFileMessage message)
        {
            try
            {
                using var fileStream = File.OpenRead(mergedFilePath);
                var args = new UploadFileHandlerArgs
                {
                    FileStream = fileStream,
                    FileName = message.FileName,
                    ContentType = message.ContentType,
                    ContentLength = fileStream.Length,
                    FilePath = mergedFilePath,
                };
                return await _telegramService.UploadFileToStrorageAsync(args, message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UploadToTelegramAsync: {ex.Message}");
                throw;
            }
        }

        private static void CleanUpPushFile(string filePath)
        {
            Console.WriteLine($"cleaning up file: {filePath}");
            Directory.Delete(filePath, true);
            Console.WriteLine($"file cleaned up: successfully");
        }

        private async Task UpdateDatabaseAsync(Guid id, TeleFileEntity? teleFileEntity)
        {
            if (teleFileEntity is null)
            {
                _logger.LogError($"File {id} not found");
                return;
            }

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IEStoreDbContext>();
                _logger.LogInformation($"Updating file {id} to {FileStatus.Uploaded}");
                var file = await context.TeleFileEntities.FindAsync(id);
                if (file is null)
                {
                    _logger.LogError($"File {id} not found");
                    return;
                }
                file.FileStatus = FileStatus.Uploaded;
                file.FileSize = teleFileEntity.FileSize;
                file.FileId = teleFileEntity.FileId;
                file.MessageId = teleFileEntity.MessageId;
                file.AccessHash = teleFileEntity.AccessHash;
                file.Flags = teleFileEntity.Flags;
                file.FileReference = teleFileEntity.FileReference;
                file.Width = teleFileEntity.Width;
                file.Height = teleFileEntity.Height;
                file.DcId = teleFileEntity.DcId;
                file.Thumbnail = teleFileEntity.Thumbnail;
                await context.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating database: {ex.Message}");
                throw;
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
                _logger.LogError($"Error during RabbitMQService Dispose: {ex.Message}");
            }
            GC.SuppressFinalize(this);
        }
    }
} 