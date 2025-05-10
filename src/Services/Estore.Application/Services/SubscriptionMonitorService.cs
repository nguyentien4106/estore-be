using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Auth.Models;

namespace EStore.Application.Services;

public class SubscriptionMonitorService(IServiceProvider serviceProvider, ILogger<SubscriptionMonitorService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Subscription monitor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Calculate time until next run at 23:59
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(now.TimeOfDay >= new TimeSpan(23, 59, 0) ? 1 : 0)
                    .Add(new TimeSpan(23, 59, 0));
                var delay = nextRun - now;

                logger.LogInformation("Next subscription check scheduled at {Time}", nextRun);
                await Task.Delay(delay, stoppingToken);

                // Process expired subscriptions
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IEStoreDbContext>();

                now = DateTime.UtcNow; // Update current time after delay

                var expiredSubscriptions = await dbContext.Subscriptions
                    .Where(s => s.IsActive && s.EndDate < now)
                    .ToListAsync(stoppingToken);

                foreach (var sub in expiredSubscriptions)
                {
                    var user = await dbContext.Users.FindAsync(sub.UserId);
                    if (user != null)
                    {
                        user.AccountType = AccountType.Free;
                    }

                    sub.IsActive = false;
                }

                await dbContext.CommitAsync(stoppingToken);
                logger.LogInformation("Checked subscriptions at {Time}", now);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking subscriptions.");
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken); // Wait a bit before retrying after error
            }
        }
    }
}
