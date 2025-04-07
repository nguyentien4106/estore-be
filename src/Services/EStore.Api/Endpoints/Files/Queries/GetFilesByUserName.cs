using BuildingBlocks.Models;
using BuildingBlocks.Pagination;
using Carter;
using EStore.Application.Files.Queries.GetFilesByUserName;

namespace EStore.Api.Endpoints.Files.Queries;

public class GetFilesByUserName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/users/{userName}", async (string userName, [AsParameters]PaginationRequest request, ISender sender) =>
        {
            var command = new GetFilesByUserNameQuery(userName, request);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<PaginatedResult<FileEntityResponse>>>(StatusCodes.Status200OK)
        .WithName("GetFilesByUserName")
        .WithTags("GetFilesByUserName")
        .RequireAuthorization();
    }
}
