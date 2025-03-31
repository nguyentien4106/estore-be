using BuildingBlocks.Models;
using Carter;
using Estore.Application.Store.Queries.GetImagesByUserName;

namespace EStore.Api.Endpoints.Store;

public class GetImagesByUserName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/store/images/users/{userName}", async (string userName, ISender sender) =>
        {
            var command = new GetImagesByUserNameQuery(userName);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<List<R2File>>>(StatusCodes.Status200OK)
        .WithName("GetImagesByUserName")
        .WithTags("GetImagesByUserName");
    }
}
