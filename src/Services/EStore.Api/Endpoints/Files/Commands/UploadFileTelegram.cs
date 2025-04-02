using BuildingBlocks.Models;
using Carter;
using Estore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class UploadFileTelegram : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/telegram", async ([FromForm] UploadFileRequest request, [FromServices] ITelegramService services, ISender sender) =>
        {
            await services.SendFormFileToChannelAsync(request.File, "Caption " + request.UserName);

            return Results.Ok();
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<R2File>>(StatusCodes.Status201Created)
        .WithName("UploadFileTelegram")
        .WithTags("UploadFileTelegram")
        .DisableAntiforgery();
    }
}
