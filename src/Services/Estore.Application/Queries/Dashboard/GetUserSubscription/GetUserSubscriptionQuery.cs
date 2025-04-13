using EStore.Application.Models.Dashboard;

namespace EStore.Application.Queries.Dashboard.GetUserSubscription;

public class GetUserSubscriptionQuery : IQuery<AppResponse<List<UserSubscriptionDto>>>
{
    public string UserName { get; set; }
} 