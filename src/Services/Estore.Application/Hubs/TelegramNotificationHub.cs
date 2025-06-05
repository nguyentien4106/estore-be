using Microsoft.AspNetCore.SignalR;

namespace EStore.Application.Hubs
{
    public class TelegramNotificationHub : Hub<ITelegramNotificationClient>
    {
        // You can add server-side hub methods here if needed, for example:
        // - Client registration for specific notifications
        // - Methods callable by clients
        // For now, it will primarily be used to dispatch messages via IHubContext
    }
} 