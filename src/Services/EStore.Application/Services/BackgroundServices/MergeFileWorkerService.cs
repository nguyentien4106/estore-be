using EStore.Application.Services.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EStore.Application.Services.BackgroundServices;

public class MergeFileWorkerService(ILogger<MergeFileWorkerService> logger, IRabbitMQService rabbitMQService) : BackgroundService
{
    private readonly ILogger<MergeFileWorkerService> _logger = logger;
    private readonly IRabbitMQService _rabbitMQService = rabbitMQService;
    private const string ConsumerTag = "merge_file_worker";
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MergeFileWorkerService is starting.");

        stoppingToken.Register(() => 
            _logger.LogInformation("MergeFileWorkerService is stopping."));

        try
        {
            await _rabbitMQService.MergeFileConsumerAsync(ConsumerTag);
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
