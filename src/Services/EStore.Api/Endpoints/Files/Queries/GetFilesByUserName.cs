using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Queries.GetFilesByUserName;

namespace EStore.Api.Endpoints.Files.Queries;

public class GetFilesByUserName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/users/{userName}", async (string userName, ISender sender) =>
        {
            var command = new GetFilesByUserNameQuery(userName);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<List<R2File>>>(StatusCodes.Status200OK)
        .WithName("GetFilesByUserName")
        .WithTags("GetFilesByUserName");
    }
}
