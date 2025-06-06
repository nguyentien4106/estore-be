using EStore.Application.Services.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EStore.Application.Services.BackgroundServices
{
    public class PushFileWorkerService(ILogger<PushFileWorkerService> logger, IRabbitMQService rabbitMQService) : BackgroundService
    {
        private readonly ILogger<PushFileWorkerService> _logger = logger;
        private readonly IRabbitMQService _rabbitMQService = rabbitMQService;
        private const string ConsumerTag = "push_file_worker";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MergeFileWorkerService is starting.");

            stoppingToken.Register(() => 
                _logger.LogInformation("MergeFileWorkerService is stopping."));

            try
            {
                await _rabbitMQService.PushFileConsumerAsync(ConsumerTag);
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
