using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Commands.DeleteImage;

namespace EStore.Api.Endpoints.Files;

public class DeleteFile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/store/files", async (string fileName, ISender sender) =>
        {
            var command = new DeleteImageCommand(fileName);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<R2File>>(StatusCodes.Status201Created)
        .WithName("DeleteFile")
        .WithTags("DeleteFile");
    }
}
