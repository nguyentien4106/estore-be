using EStore.Application.Models.Dashboard;
using EStore.Application.Queries.Dashboard.GetUserSubscription;

namespace EStore.Api.Endpoints.Dashboard;

public class GetUserSubscription : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/subscription/{userName}", async (string userName, ISender sender) =>
        {
            var query = new GetUserSubscriptionQuery
            {
                UserName = userName
            };
            var result = await sender.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetUserSubscription")
        .WithTags("Dashboard")
        .RequireAuthorization()
        .Produces<AppResponse<List<UserSubscriptionDto>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithDescription("Get user's subscription information")
        .WithSummary("Get subscription dashboard data");
    }
} 