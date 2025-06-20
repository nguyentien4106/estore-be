﻿using System.Security.Cryptography;
using EStore.Application.Commands.Files.UploadFileMultipart;
using EStore.Application.Constants;
using EStore.Application.Models.Files;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class UploadFileMultipart : ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/multipart", async (
            [FromForm] UploadFileMultipartRequest request, 
            ISender sender,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new UploadFileMultipartCommand(
                request.File,
                request.ChunkIndex, 
                request.TotalChunks, 
                request.FileName, 
                request.UserId, 
                request.FileId, 
                request.UserName, 
                request.ContentType
            );

            var result = await sender.Send(command, cancellationToken);

            return Results.Ok(result);
            
        })
        .Accepts<UploadFileMultipartRequest>("multipart/form-data")
        .Produces<AppResponse<FileEntityResult>>(StatusCodes.Status201Created)
        .WithName("UploadFileMultipart")
        .WithTags("UploadFile")
        .DisableAntiforgery()
        .WithMetadata(new RequestFormLimitsAttribute { MultipartBodyLengthLimit = FileSizeLimits.ProTierLimit }); // 5 GB
    }
}