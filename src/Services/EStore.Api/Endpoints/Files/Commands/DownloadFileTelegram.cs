using BuildingBlocks.Models;
using Carter;
using Estore.Application.Services.Telegram;
using EStore.Application.Data;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class DownloadFileTelegram : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/telegram/download/{id}", async (Guid id, [FromServices]IEStoreDbContext context, [FromServices]ITelegramService services, ISender sender) =>
        {
            var fileLocation = await context.TeleFilesLocations.FindAsync(id);  
            if(fileLocation == null){
                return Results.NotFound();
            }
            var result = await services.DownloadFileAsync(fileLocation);
            if(result.Succeed){
                return Results.Ok(result.Data);
            }
            return Results.BadRequest(result.Message);
        })
        .Produces<AppResponse<FileInformationDto>>(StatusCodes.Status201Created)
        .WithName("DownloadFileTelegram")
        .WithTags("DownloadFileTelegram")
        .DisableAntiforgery();
    }
}
