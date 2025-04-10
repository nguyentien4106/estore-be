using Carter;
using EStore.Application.Factories;
using EStore.Domain.Enums.Files;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace EStore.Api.Endpoints.Files.Commands;

public class DownloadFileTest : ICarterModule 
{
    private JsonTypeInfo _jsonSerializerOptions;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/download1", async (HttpContext context, [FromBody] DownloadFileRequest request, ISender sender) =>
        {
            var command = CommandHandlerFactory.GetDownloadFileCommand(request);
            var result = await sender.Send(command);
            
            if(!result.Succeed || result.Data is null){
                return Results.Ok(result);
            }

            // Extract the file stream, content type, and file name
            var fileStream = result.Data.FileStream;
            var contentType = result.Data.ContentType ?? "application/octet-stream";
            var fileName = result.Data.FileName ?? "downloaded-file";

            // Get the file length (we'll need to seek to the end and back to get the length)
            var originalPosition = fileStream.Position;
            fileStream.Seek(0, SeekOrigin.End);
            var fileLength = fileStream.Position;
            fileStream.Seek(originalPosition, SeekOrigin.Begin);

            try
            {
                // Check if the request includes a Range header
                var rangeHeader = context.Request.Headers["Range"].FirstOrDefault();
                if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
                {
                    // Parse the range header (e.g., "bytes=0-1023")
                    var ranges = rangeHeader.Replace("bytes=", "").Split('-');
                    var start = long.Parse(ranges[0]);
                    var end = ranges.Length > 1 && !string.IsNullOrEmpty(ranges[1]) ? long.Parse(ranges[1]) : fileLength - 1;

                    if (start < 0 || end >= fileLength || start > end)
                    {
                        context.Response.StatusCode = StatusCodes.Status416RangeNotSatisfiable;
                        context.Response.ContentType = "application/json";
                        var errorResponse = new { error = "Invalid range.", code = "INVALID_RANGE" };
                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, _jsonSerializerOptions));
                        return Results.StatusCode(StatusCodes.Status416RangeNotSatisfiable);
                    }

                    // Set headers for partial content
                    context.Response.StatusCode = StatusCodes.Status206PartialContent;
                    context.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileLength}");
                    context.Response.Headers.Add("Accept-Ranges", "bytes");
                    context.Response.Headers.Add("Content-Length", (end - start + 1).ToString());
                    context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                    context.Response.ContentType = contentType;

                    // Stream the requested range
                    fileStream.Seek(start, SeekOrigin.Begin);
                    var length = end - start + 1;
                    var buffer = new byte[4096];
                    long bytesRemaining = length;
                    while (bytesRemaining > 0)
                    {
                        int bytesToRead = (int)Math.Min(buffer.Length, bytesRemaining);
                        int bytesRead = await fileStream.ReadAsync(buffer, 0, bytesToRead);
                        if (bytesRead == 0) break; // End of stream
                        await context.Response.Body.WriteAsync(buffer, 0, bytesRead);
                        bytesRemaining -= bytesRead;
                    }

                    return Results.StatusCode(StatusCodes.Status206PartialContent);
                }

                // No range request: Serve the entire file
                context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                context.Response.Headers.Add("Content-Length", fileLength.ToString());
                context.Response.Headers.Add("Accept-Ranges", "bytes");
                context.Response.ContentType = contentType;

                // Stream the file in chunks
                var streamBuffer = new byte[4096];
                int bytesReadStream;
                while ((bytesReadStream = await fileStream.ReadAsync(streamBuffer, 0, streamBuffer.Length)) > 0)
                {
                    await context.Response.Body.WriteAsync(streamBuffer, 0, bytesReadStream);
                }

                // Explicitly return a result after streaming
                return Results.StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var errorResponse = new { error = $"Error processing file: {ex.Message}", code = "STREAMING_ERROR" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, _jsonSerializerOptions));
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                // Dispose of the file stream
                await fileStream.DisposeAsync();
            }
        })
        .WithName("DownloadFileTest")
        .WithTags("DownloadFileTest");
    }
}
