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
using Microsoft.AspNetCore.SignalR;
using EStore.Application.Hubs;
using EStore.Application.Extensions;

namespace EStore.Application.Services.RabbitMQ
{

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
        private readonly IHubContext<TelegramNotificationHub, ITelegramNotificationClient> _hubContext;

        public RabbitMQService(
            RabbitMQConfiguration rabbitMQOptions,
            ITelegramService telegramService,
            IServiceScopeFactory factory,
            IHubContext<TelegramNotificationHub, ITelegramNotificationClient> hubContext,
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
            _hubContext = hubContext;
        }

        public async Task<bool> ProducerAsync(string queueName, string message)
        {
            if (_connection is null || !_connection.IsOpen || _producerChannel is null)
            {
                _logger.LogError("RabbitMQ channel is not available for producer.");
                return false;
            }

            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                var properties = new BasicProperties() { Persistent = true };

                await _producerChannel.BasicPublishAsync(
                    exchange: QueueConstants.ExchangeName,
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message via ProducerAsync: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> ProducerAsync(string queueName, object data)
        {
            if (_connection is null || !_connection.IsOpen || _producerChannel is null)
            {
                _logger.LogError("RabbitMQ channel is not available for producer.");
                return false;
            }

            try
            {
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
                var properties = new BasicProperties() { Persistent = true };

                await _producerChannel.BasicPublishAsync(
                    exchange: QueueConstants.ExchangeName,
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message via ProducerAsync: {Message}", ex.Message);
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
                _logger.LogError(ex, "Error starting consumer [{ConsumerTag}]: {Message}", consumerTag, ex.Message);
                throw;
            }
        }

        private async Task HandleReceivedMessageMergeFileAsync(object model, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<ChunkMessage>(Encoding.UTF8.GetString(body));
                if (message is null)
                {
                    _logger.LogError("HandleReceivedMessageMergeFileAsync Invalid message: {Message}", Encoding.UTF8.GetString(body));
                    return;
                }

                var messagesForFile = _messageStore.GetOrAdd(message.FileId, _ => []);
                messagesForFile.Add((message, ea.DeliveryTag));

                if (!IsAllChunksReceived(messagesForFile, message.TotalChunks))
                {
                    return;
                }

                var fileId = message.Id != Guid.Empty ? message.Id : messagesForFile.FirstOrDefault(item => item.Message.Id != Guid.Empty).Message.Id;
                var mergedFilePath = await MergeChunksAsync(message);
                var pushFileMessage = new PushingFileMessage
                {
                    FilePath = mergedFilePath,
                    FileId = fileId,
                    UserId = message.UserId,
                    FileName = message.FileName,
                    ContentType = message.ContentType
                };

                await ProducerAsync(QueueConstants.PushFileQueue, pushFileMessage);
                await CleanUpPartFiles(_mergeFileConsumerChannel, message, messagesForFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("HandleReceivedMessageMergeFileAsync error: {Message}", ex.Message);
                await _mergeFileConsumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        }

        private static bool IsAllChunksReceived(ConcurrentBag<(ChunkMessage, ulong)> messagesForFile, int totalChunks)
        {
            return messagesForFile.Count == totalChunks;
        }

        private async Task<string> MergeChunksAsync(ChunkMessage message)
        {
            try
            {
                var filePath = FileHelper.GetTempsFilePath(message.UserId, message.FileId);
                var chunks = Directory.GetFiles(filePath);
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
                _logger.LogError(ex, "Error in MergeChunksAsync: {Message}", ex.Message);
                throw;
            }
        }

        private async Task CleanUpPartFiles(IChannel channel, ChunkMessage message, ConcurrentBag<(ChunkMessage, ulong)> messagesForFile)
        {
            Directory.Delete(FileHelper.GetTempsFilePath(message.UserId, message.FileId), true);

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
                _logger.LogError("PushFileConsumerAsync consumer [{ConsumerTag}]: {Message}", consumerTag, ex.Message);
                throw;
            }
        }

        private async Task HandleReceivedMessagePushFileAsync(object model, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<PushingFileMessage>(Encoding.UTF8.GetString(body));
                if (message is null)
                {
                    _logger.LogError("HandleReceivedMessagePushFileAsync Invalid message: {Message}", Encoding.UTF8.GetString(body));
                    return;
                }

                await _pushFileConsumerChannel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                var teleFileEntity = await UploadFileAsync(message.FilePath, message);
                if (teleFileEntity.Succeed)
                {
                    var file = await UpdateFileInformationAsync(message.FileId, teleFileEntity.Data);
                    await _hubContext.Clients.All.ReceiveUploadCompleted(message.FileId.ToString(), file?.ToFileEntityResponse() ?? null);
                    CleanUpPushFile(message.FilePath);
                }
                else
                {
                    _logger.LogError("Error uploading file {FileId} to Telegram: {Message}", message.FileId, teleFileEntity.Message);
                    await _pushFileConsumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("HandleReceivedMessagePushFileAsync: {Message}", ex.Message);
                await _pushFileConsumerChannel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        }

        private async Task<AppResponse<TeleFileEntity>> UploadFileAsync(string mergedFilePath, PushingFileMessage message)
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
                    FileId = message.FileId
                };
                return await _telegramService.UploadFileArgsAsync(args, message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UploadToTelegramAsync: {Message}", ex.Message);
                throw;
            }
        }

        private static void CleanUpPushFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private async Task<TeleFileEntity?> UpdateFileInformationAsync(Guid id, TeleFileEntity? teleFileEntity)
        {
            if (teleFileEntity is null)
            {
                _logger.LogError("File {Id} not found", id);
                return null;
            }
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IEStoreDbContext>();
                var file = await context.TeleFileEntities.FindAsync(id);
                if (file is null)
                {
                    _logger.LogError("File {Id} not found", id);
                    return null;
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
                return file;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating database: {Message}", ex.Message);
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
                _logger.LogError("Error during RabbitMQService Dispose: {Message}", ex.Message);
            }
            GC.SuppressFinalize(this);
        }
    }
} 