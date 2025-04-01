using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Queries.GetImagesByUserName;

namespace EStore.Api.Endpoints.Files;

public class GetFilesByUserName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/store/files/users/{userName}", async (string userName, ISender sender) =>
        {
            var command = new GetImagesByUserNameQuery(userName);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<List<R2File>>>(StatusCodes.Status200OK)
        .WithName("GetFilesByUserName")
        .WithTags("GetFilesByUserName");
    }
}
