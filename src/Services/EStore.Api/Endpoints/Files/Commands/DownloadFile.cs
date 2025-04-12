using Carter;
using Microsoft.AspNetCore.Mvc;
using EStore.Application.Commands.Files.DownloadFile;
using EStore.Application.DesignPatterns.Factories;

namespace EStore.Api.Endpoints.Files.Commands;

public class DownloadFile : ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/download", async ([FromBody] DownloadFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetDownloadFileCommand(request);
            var result = await sender.Send(command);
            
            if(!result.Succeed || result.Data is null){
                return Results.Ok(result);
            }

            try
            {
                return Results.File(
                    fileStream: result.Data.FileStream,
                    contentType: result.Data.ContentType,
                    fileDownloadName: result.Data.FileName,
                    enableRangeProcessing: true
                );
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error processing file: {ex.Message}");
            }
        })
        .WithName("DownloadFile")
        .WithTags("DownloadFile");
    }
}
