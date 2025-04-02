using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Commands.DeleteFile;

namespace EStore.Api.Endpoints.Files.Commands;

public class DeleteFile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/files", async (Guid Id, ISender sender) =>
        {
            var command = new DeleteFileCommand(Id);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<R2File>>(StatusCodes.Status201Created)
        .WithName("DeleteFile")
        .WithTags("DeleteFile");
    }
}
