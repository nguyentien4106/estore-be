using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Commands.DeleteFile;
using Estore.Application.Files.Commands.DeleteFileTelegram;
using Estore.Domain.Enums.Files;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class DeleteFile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/files", async ([FromQuery]Guid id, [FromQuery] StorageSource storageSource, ISender sender) =>
        {
            AppResponse<FileInformationDto> result;
            if (storageSource == StorageSource.R2)
            {
                var command = new DeleteFileCommand(id);
                result = await sender.Send(command);
            }
            else
            {
                var command = new DeleteFileTelegramCommand(id);
                result = await sender.Send(command);
            }

            return Results.Ok(result);
        })
        .Produces<AppResponse<FileInformationDto>>(StatusCodes.Status201Created)
        .WithName("DeleteFile")
        .WithTags("DeleteFile");
    }
}
