using BuildingBlocks.Models;
using Carter;
using Estore.Application.Dtos.Store;
using Estore.Application.Files.Commands.UploadFile;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files;

public class UploadFile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/store/files", async ([FromForm] StoreImageRequest request, ISender sender) =>
        {
            var command = new UploadFileCommand(UserName: request.UserName, File: request.File);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<R2File>>(StatusCodes.Status201Created)
        .WithName("UploadFile")
        .WithTags("UploadFile")
        .DisableAntiforgery();
    }
}
