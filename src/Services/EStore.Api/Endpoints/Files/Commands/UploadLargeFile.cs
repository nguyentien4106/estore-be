using BuildingBlocks.Models;
using Carter;
using EStore.Application.Constants;
using EStore.Application.Files.Commands.UploadLargeFile;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Globalization;

namespace EStore.Api.Endpoints.Files.Commands;

public class UploadLargeFile : ICarterModule 
{
    private static readonly FormOptions _defaultFormOptions = new FormOptions();

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/large", async (HttpRequest request, HttpResponse response, ISender sender) =>
        {

           
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AppResponse<FileEntityResponse>>(StatusCodes.Status201Created)
        .WithName("UploadLargeFile")
        .WithTags("UploadLargeFile")
        .DisableAntiforgery();
    }

    
}
