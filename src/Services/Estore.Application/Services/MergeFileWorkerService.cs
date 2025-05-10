using EStore.Application.Services.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EStore.Application.Services
{
    public class MergeFileWorkerService : BackgroundService
    {
        private readonly ILogger<MergeFileWorkerService> _logger;
        private readonly IRabbitMQService _rabbitMQService;
        private const string ConsumerTag = "merge_file_worker"; // Unique tag for this consumer

        public MergeFileWorkerService(ILogger<MergeFileWorkerService> logger, IRabbitMQService rabbitMQService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MergeFileWorkerService is starting.");

            stoppingToken.Register(() => 
                _logger.LogInformation("MergeFileWorkerService is stopping."));

            try
            {
                // Note: The current IRabbitMQService.ConsumerAsync consumes from a hardcoded "hello" queue
                // and uses autoAck. For actual file merging, you'd want to:
                // 1. Consume from the configured queue (e.g., the one used by the producer for file chunk messages).
                // 2. Implement message processing logic (downloading chunks, merging them).
                // 3. Use manual acknowledgment (autoAck: false) and acknowledge messages only after successful processing.
                // 4. Handle potential errors during message processing and decide on nack/requeue/dead-lettering.

                // The RabbitMQService.ConsumerAsync as per your latest changes returns Task<bool>.
                // We'll call it and log its outcome. In a real scenario, you might loop or have more sophisticated logic based on the return.
                var result = await _rabbitMQService.ConsumerAsync(ConsumerTag);

                if (result.Succeed)
                {
                    _logger.LogInformation($"Consumer '{ConsumerTag}' started successfully. Waiting for messages to merge files.");
                    // Keep the service alive while the consumer is running in the background
                    // The AsyncEventingBasicConsumer runs on its own threads managed by the RabbitMQ client library.
                    // The stoppingToken will signal when to shut down.
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Keep alive, check periodically
                    }
                }
                else
                {
                    _logger.LogError($"Consumer '{ConsumerTag}' failed to start. The service will not process messages.");
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is signaled, an OperationCanceledException is thrown by Task.Delay or other cancellable operations.
                _logger.LogInformation("MergeFileWorkerService execution was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred in MergeFileWorkerService.");
            }
            finally
            {
                _logger.LogInformation("MergeFileWorkerService has finished execution.");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MergeFileWorkerService is stopping via StopAsync.");
            // Perform any cleanup if necessary, though RabbitMQService handles its own Dispose
            await base.StopAsync(stoppingToken);
        }
    }
}
