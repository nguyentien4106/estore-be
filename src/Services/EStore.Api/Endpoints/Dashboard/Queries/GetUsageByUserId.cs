using BuildingBlocks.Models;
using Carter;
using Estore.Application.Dashboard.GetUsageByUserId;
using Estore.Application.Dtos.Dashboard;

namespace EStore.Api.Endpoints.Dashboard.Queries;

public class GetUsageByUserId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/usage/{userId}", async (string userId, ISender sender) =>
        {
            var query = new GetUsageByUserIdQuery(userId);
            var result = await sender.Send(query);

            return Results.Ok(result);
        })
        .Produces<AppResponse<List<StorageUsageDto>>>(StatusCodes.Status200OK)
        .WithName("GetUsageByUserId")
        .WithTags("GetUsageByUserId")
        .RequireAuthorization();
    }
}
