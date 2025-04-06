using Carter;
using EStore.Application.Factories;
using EStore.Domain.Enums.Files;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files.Commands;

public class DownloadFile : ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/download", async ([FromBody] DownloadFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetDownloadFileCommand(request);
            var result = await sender.Send(command);
            
            if(!result.Succeed){
                return Results.Ok(result);
            }

            if(request.StorageSource == StorageSource.R2){
                return Results.Ok(result);
            }

            if (!File.Exists(result.Data.FilePath))
            {
                return Results.NotFound("FilePath not found.");
            }

            var fileName = Path.GetFileName(result.Data.FilePath);
            
            var file = Results.File(
                fileStream: File.OpenRead(result.Data.FilePath),
                contentType: result.Data.ContentType,
                fileDownloadName: fileName,
                enableRangeProcessing: true
            );
            
            return file;
        })
        .WithName("DownloadFile")
        .WithTags("DownloadFile");
    }
}
