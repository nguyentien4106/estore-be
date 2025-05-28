using EStore.Application.Services.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EStore.Application.Services.BackgroundServices
{
    public class PushFileWorkerService : BackgroundService
    {
        private readonly ILogger<PushFileWorkerService> _logger;
        private readonly IRabbitMQService _rabbitMQService;
        private const string ConsumerTag = "push_file_worker"; // Unique tag for this consumer

        public PushFileWorkerService(ILogger<PushFileWorkerService> logger, IRabbitMQService rabbitMQService)
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
                await _rabbitMQService.PushFileConsumerAsync(ConsumerTag);
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
            await base.StopAsync(stoppingToken);
        }
    }
}
