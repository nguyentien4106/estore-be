using EStore.Application.Queries.Files.GetR2FilePreview;
using MediatR;

namespace EStore.Api.Endpoints.Files.Queries;

public class GetR2FilePreview : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{id}/preview", async (Guid id, ISender sender) =>
        {
            var query = new GetR2FilePreviewQuery(id);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetR2FilePreview")
        .WithTags("Files");
    }
} 