using BuildingBlocks.Models;
using Carter;
using Estore.Application.Factories;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class UploadFile : ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files", async ([FromForm] UploadFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetUploadFileCommand(request);
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
