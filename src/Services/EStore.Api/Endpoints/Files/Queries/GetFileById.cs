using BuildingBlocks.Models;
using Carter;
using Estore.Application.Files.Queries.GetFileById;

namespace EStore.Api.Endpoints.Files.Queries;

public class GetFileById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetFileByIdQuery(id);
            var result = await sender.Send(query);

            return Results.Ok(result);
        })
        .Produces<AppResponse<FileInformationDto>>(StatusCodes.Status200OK)
        .WithName("GetFileById")
        .WithTags("GetFileById");
    }
}
