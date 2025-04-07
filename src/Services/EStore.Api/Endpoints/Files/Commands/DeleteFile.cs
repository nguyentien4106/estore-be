using BuildingBlocks.Models;
using Carter;
using EStore.Application.Factories;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class DeleteFile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/files", async ([FromBody]DeleteFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetDeleteFileCommand(request);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<Guid>>(StatusCodes.Status201Created)
        .WithName("DeleteFile")
        .WithTags("DeleteFile")
        .RequireAuthorization();
    }
}
