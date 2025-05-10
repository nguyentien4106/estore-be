using EStore.Application.Models.Dashboard;
using EStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EStore.Application.Queries.Dashboard.GetUserSubscription;

public class GetUserSubscriptionHandler(IEStoreDbContext context) : IQueryHandler<GetUserSubscriptionQuery, AppResponse<List<UserSubscriptionDto>>>
{
    public async Task<AppResponse<List<UserSubscriptionDto>>> Handle(GetUserSubscriptionQuery query, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName == query.UserName, cancellationToken);

        if (user == null)
        {
            return AppResponse<List<UserSubscriptionDto>>.NotFound("User", query.UserName);
        }

        var subscriptions = await context.Subscriptions.Where(sub => sub.UserId == user.Id)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        var subscriptionDtos = subscriptions.Select(sub => new UserSubscriptionDto
        {
            AccountType = user.AccountType,
            IsActive = sub.IsActive,
            StartDate = sub.StartDate,
            EndDate = sub.EndDate,
        }).ToList();

        return AppResponse<List<UserSubscriptionDto>>.Success(subscriptionDtos);
    }
} 