using EStore.Application.Models.Dashboard;
using EStore.Application.Queries.Dashboard;

namespace EStore.Api.Endpoints.Dashboard;

public class GetUserStorage : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/storage/{userName}", async (string userName, ISender sender) =>
        {
            var query = new GetUserStorageQuery(userName);
            var result = await sender.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetUserStorage")
        .WithTags("Dashboard")
        .RequireAuthorization()
        .Produces<AppResponse<UserStorageDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithDescription("Get user's storage usage information")
        .WithSummary("Get storage dashboard data");
    }
} 