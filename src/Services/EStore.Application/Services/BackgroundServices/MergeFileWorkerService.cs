using EStore.Application.Services.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EStore.Application.Services.BackgroundServices
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
                await _rabbitMQService.MergeFileConsumerAsync(ConsumerTag);
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
