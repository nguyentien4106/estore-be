using BuildingBlocks.Models;
using Carter;
using Estore.Application.Dtos.Store;
using Estore.Application.Services;
using Estore.Application.Store.Commands.StoreImage;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Store;

public class UploadFileTelegram : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/store/telegram/files", async ([FromForm]StoreImageRequest request, [FromServices]ITelegramService services , ISender sender) =>
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
