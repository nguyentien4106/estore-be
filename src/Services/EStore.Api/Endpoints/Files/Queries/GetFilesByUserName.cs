using BuildingBlocks.Pagination;
using EStore.Application.Queries.Files.GetFilesByUserName;
using EStore.Application.Models.Files;

namespace EStore.Api.Endpoints.Files.Queries;

public class GetFilesByUserName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/users/{userName}", async (string userName, [AsParameters]GetFilesByUserNameRequest request, ISender sender) =>
        {
            var command = new GetFilesByUserNameQuery(userName, request);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<PaginatedResult<FileEntityResult>>>(StatusCodes.Status200OK)
        .WithName("GetFilesByUserName")
        .WithTags("GetFilesByUserName")
        .RequireAuthorization();
    }
}
