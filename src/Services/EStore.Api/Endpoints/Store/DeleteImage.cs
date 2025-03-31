using BuildingBlocks.Models;
using Carter;
using Estore.Application.Dtos.Store;
using Estore.Application.Store.Commands.StoreImage;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Store;

public class UploadImage : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/store/images", async ([FromForm] StoreImageRequest request, ISender sender) =>
        {
            var command = new StoreImageCommand(UserName: request.UserName, File: request.File);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<string>>(StatusCodes.Status201Created)
        .WithName("UploadFile")
        .WithTags("FileUpload")
        .DisableAntiforgery();
    }
}
