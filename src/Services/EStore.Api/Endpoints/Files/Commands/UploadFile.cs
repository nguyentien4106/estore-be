using BuildingBlocks.Models;
using Carter;
using EStore.Application.Factories;
using EStore.Domain.Models.Base;
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
        .Produces<AppResponse<FileEntityResponse>>(StatusCodes.Status201Created)
        .WithName("UploadFile")
        .WithTags("UploadFile")
        .RequireAuthorization()
        .DisableAntiforgery();
    }
}
