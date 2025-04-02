using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Commands.UploadFileTelegram;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class UploadFileTelegram : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/telegram", async ([FromForm] UploadFileRequest request, ISender sender) =>
        {
            var command = new UploadFileTelegramCommand(request.File, request.UserName);
            
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<FileInformationDto>>(StatusCodes.Status201Created)
        .WithName("UploadFileTelegram")
        .WithTags("UploadFileTelegram")
        .DisableAntiforgery();
    }
}
