using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Options;

namespace EStore.Application.Services.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName = "chunk";
        private IConnection? _connection;

        private readonly IChannel _producerChannel;
        private readonly IChannel _consumerChannel;

        private const string _exchangeName = "file-exchange";
        public RabbitMQService(RabbitMQConfiguration rabbitMQOptions)
        {
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

            _consumerChannel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
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
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    try
                    {
                        await ProcessMessageAsync(message); // Extracted processing logic
                        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
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

        private async Task ProcessMessageAsync(string message)
        {
            Console.WriteLine($" [x] Received {message}");
            // Add your processing logic here (e.g., update database, call S3)
            await Task.CompletedTask;
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