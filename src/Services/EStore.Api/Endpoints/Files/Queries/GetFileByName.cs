using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Queries.GetFileByName;

namespace EStore.Api.Endpoints.Files.Queries;

public class GetFileByName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/files", async (string fileName, ISender sender) =>
        {
            var command = new GetFileByNameQuery(fileName);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<R2File>>(StatusCodes.Status200OK)
        .WithName("GetFileByName")
        .WithTags("GetFileByName");
    }
}
