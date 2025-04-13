using EStore.Application.Commands.Files.UploadFile;
using EStore.Application.Constants;
using EStore.Application.DesignPatterns.Factories;
using EStore.Application.Models.Files;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class UploadFile : ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Free tier: 20 MB limit
        app.MapPost("/files/free", async ([FromForm] UploadFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetUploadFileCommand(request);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<FileEntityResult>>(StatusCodes.Status201Created)
        .WithName("UploadFileFree")
        .WithTags("UploadFile")
        .DisableAntiforgery()
        .RequireAuthorization("FreeTierFileSizeLimit"); // 20 MB

        // Pro tier: 2 GB limit
        app.MapPost("/files/pro", async ([FromForm] UploadFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetUploadFileCommand(request);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<FileEntityResult>>(StatusCodes.Status201Created)
        .WithName("UploadFilePro")
        .WithTags("UploadFile")
        .DisableAntiforgery()
        .RequireAuthorization("RequirePro", "ProTierFileSizeLimit"); // 2 GB

        // Plus tier: 5 GB limit
        app.MapPost("/files/plus", async ([FromForm] UploadFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetUploadFileCommand(request);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<FileEntityResult>>(StatusCodes.Status201Created)
        .WithName("UploadFilePlus")
        .WithTags("UploadFile")
        .DisableAntiforgery()
        .RequireAuthorization("RequirePlus")
        .WithMetadata(new RequestFormLimitsAttribute { MultipartBodyLengthLimit = FileSizeLimits.PlusTierLimit }); // 5 GB
    }
}