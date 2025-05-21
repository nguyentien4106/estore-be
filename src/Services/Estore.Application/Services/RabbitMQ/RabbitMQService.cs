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

namespace EStore.Application.Services.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName = "chunk";
        private const string _exchangeName = "file-exchange";

        private IConnection? _connection;
        private readonly IChannel _producerChannel;
        private IChannel _consumerChannel;
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
            _producerChannel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true).GetAwaiter();
            _producerChannel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null).GetAwaiter();
            _producerChannel.QueueBindAsync(_queueName, _exchangeName, "chunk").GetAwaiter();

            _logger = logger;
        }

        public async Task<bool> ProducerAsync(string message)
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
                    exchange: _exchangeName,
                    routingKey: _queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation($"Sent message to {_queueName}: {message}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message via ProducerAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<AppResponse<string>> ConsumerAsync(string consumerTag)
        {
            try
            {
                _consumerChannel = await _connection.CreateChannelAsync();
                await _consumerChannel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                await _consumerChannel.QueueBindAsync(_queueName, _exchangeName, "chunk");

                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.ReceivedAsync += HandleReceivedMessageAsync;

                var result = await _consumerChannel.BasicConsumeAsync(_queueName, autoAck: false, consumerTag, consumer);
                _logger.LogInformation($"[{consumerTag}] Consumer started on queue '{_queueName}'. Waiting for messages.");

                return AppResponse<string>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting consumer [{consumerTag}]: {ex.Message}");
                return AppResponse<string>.Error(ex.Message);
            }
        }

        private async Task HandleReceivedMessageAsync(object model, BasicDeliverEventArgs ea)
        {
            Stream? mergedFileStream = null;
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<ChunkMessage>(Encoding.UTF8.GetString(body));
                _logger.LogInformation($"Received message for chunk {message.ChunkIndex} of file {message.FileId}");

                var messagesForFile = _messageStore.GetOrAdd(message.FileId, _ => []);
                messagesForFile.Add((message, ea.DeliveryTag));

                if (messagesForFile.Count == message.TotalChunks)
                {
                    var id = message.Id != Guid.Empty ? message.Id : messagesForFile.FirstOrDefault(item => item.Message.Id != Guid.Empty).Message.Id;

                    mergedFileStream = await MergeChunksAsync(message);
                    var teleFileEntity = await UploadToTelegramAsync(mergedFileStream, message);
                    if(teleFileEntity.Succeed)
                    {
                        await UpdateDatabaseAsync(id, teleFileEntity.Data);
                        await CleanUpAsync(_consumerChannel, message, messagesForFile);
                    }
                    else {
                        _logger.LogError($"Error uploading file {message.FileId} to Telegram: {teleFileEntity.Message}");
                        await _consumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
                await _consumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
            finally
            {
                mergedFileStream?.Close();
            }
        }

        private async Task<Stream> MergeChunksAsync(ChunkMessage message)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "temps", message.UserId, message.FileId);
                var chunks = Directory.GetFiles(filePath);

                var mergedFilePath = Path.Combine(AppContext.BaseDirectory, "results", message.UserId, message.FileId, message.FileName);

                var mergedFilePathDirectory = Path.GetDirectoryName(mergedFilePath);
                if (mergedFilePathDirectory != null && !Directory.Exists(mergedFilePathDirectory))
                {
                    Directory.CreateDirectory(mergedFilePathDirectory);
                }
                
                var mergedFileStream = File.Create(mergedFilePath);
                foreach (var chunk in chunks)
                {
                    using (var chunkFileStream = File.OpenRead(chunk))
                    {
                        await chunkFileStream.CopyToAsync(mergedFileStream);
                    }
                }

                return mergedFileStream;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MergeChunksAsync: {ex.Message}");
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

        private async Task<AppResponse<TeleFileEntity>> UploadToTelegramAsync(Stream mergedFileStream, ChunkMessage message)
        {
            try
            {
                mergedFileStream.Position = 0;
                var args = new UploadFileHandlerArgs
                {
                    FileStream = mergedFileStream,
                    FileName = message.FileName,
                    ContentType = message.ContentType,
                    ContentLength = mergedFileStream.Length,
                };
                var result = await _telegramService.UploadFileToStrorageAsync(args, message.UserId);

                if (result.Succeed)
                {
                    _logger.LogInformation($"Uploaded file {message.FileId} to Telegram");
                }
                else
                {
                    _logger.LogError($"Error uploading file {message.FileId} to Telegram: {result.Message}");
                    return AppResponse<TeleFileEntity>.Error(result.Message);
                }
                return AppResponse<TeleFileEntity>.Success(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UploadToTelegramAsync: {ex.Message}");
                throw;
            }
            finally
            {
                mergedFileStream.Close();
            }
        }

        private async Task UpdateDatabaseAsync(Guid id, TeleFileEntity teleFileEntity)
        {
            try{
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IEStoreDbContext>();
                _logger.LogInformation($"Updating file {id} to {FileStatus.Uploaded}");
                var file = await context.TeleFileEntities.FindAsync(id);
                if(file is null)
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