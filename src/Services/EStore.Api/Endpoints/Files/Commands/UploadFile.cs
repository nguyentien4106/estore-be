using BuildingBlocks.Models;
using Carter;
using Estore.Application.Dtos.Files;
using Estore.Application.Files.Commands.UploadFile;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public record UploadFileRequest(IFormFile File, string UserName, int Width, int Height);

public class UploadFile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files", async ([FromForm] UploadFileRequest request, ISender sender) =>
        {
            var command = new UploadFileCommand(File: request.File, UserName: request.UserName, Width: request.Width, Height: request.Height);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<FileInformationDto>>(StatusCodes.Status201Created)
        .WithName("UploadFile")
        .WithTags("UploadFile")
        .DisableAntiforgery();
    }
}
