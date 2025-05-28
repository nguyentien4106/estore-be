using EStore.Application.Services.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EStore.Application.Services.BackgroundServices;

public abstract class BaseWorker : BackgroundService
{
    protected readonly ILogger<BaseWorker> _logger;
    protected readonly IRabbitMQService _rabbitMQService;

    public BaseWorker(ILogger<BaseWorker> logger, IRabbitMQService rabbitMQService)
    {
        _logger = logger;
        _rabbitMQService = rabbitMQService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{GetType().Name} is starting.");

        stoppingToken.Register(() =>
            _logger.LogInformation($"{GetType().Name} is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            await OnExecuteAsync();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }
    
    public abstract Task OnExecuteAsync();
}