using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Commands.UploadFileTelegram;
using Estore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class DownloadFileTelegram : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/telegram/download/{id}", async (int id, [FromServices]ITelegramService services, ISender sender) =>
        {
            var result = await services.DownloadFileAsync(id);
            return Results.Ok(result);
        })
        .Produces<AppResponse<FileInformationDto>>(StatusCodes.Status201Created)
        .WithName("DownloadFileTelegram")
        .WithTags("DownloadFileTelegram")
        .DisableAntiforgery();
    }
}
