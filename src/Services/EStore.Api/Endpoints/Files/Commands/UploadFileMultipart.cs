using System.Security.Cryptography;
using System.Text.Json;
using EStore.Application.Commands.Files.UploadFile;
using EStore.Application.Constants;
using EStore.Application.DesignPatterns.Factories;
using EStore.Application.Models.Files;
using EStore.Application.Services.RabbitMQ;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class UploadFileMultipartRequest
{
    public IFormFile File { get; set; }
    public int ChunkIndex { get; set; }
    public int TotalChunks { get; set; }
    public string FileName { get; set; }
    public string UserId { get; set; }
    public string FileId { get; set; }
}

public class UploadFileMultipart : ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        
        // Plus tier: 5 GB limit
        app.MapPost("/files/multipart", async ([FromForm] UploadFileMultipartRequest request, [FromServices] IRabbitMQService queueService) =>
        {
            var file = request.File;
            var fileId = request.FileName;
            var chunkIndex = request.ChunkIndex;
            using var stream = file.OpenReadStream();

            // Save chunk to local disk
            var tempDir = Path.Combine("temps", request.UserId, request.FileId);
            Directory.CreateDirectory(tempDir); // Ensure directory exists
            var filePath = Path.Combine(tempDir, chunkIndex.ToString());
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin); // Reset stream position
                await stream.CopyToAsync(fileStream);
            }
            // Publish to RabbitMQ
            var message = JsonSerializer.Serialize(new {
                FileId = request.FileId,
                UserId = request.UserId,
                FilePath = filePath
            });
            await queueService.ProducerAsync(message);

            return Results.Ok();
        })
        .Accepts<UploadFileMultipartRequest>("multipart/form-data")
        .Produces<AppResponse<FileEntityResult>>(StatusCodes.Status201Created)
        .WithName("UploadFileMultipart")
        .WithTags("UploadFile")
        .DisableAntiforgery()
        .WithMetadata(new RequestFormLimitsAttribute { MultipartBodyLengthLimit = FileSizeLimits.PlusTierLimit }); // 5 GB
    }
}