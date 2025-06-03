using BuildingBlocks.Pagination;
using EStore.Application.Queries.Stores.GetAllStores;
using EStore.Application.Models;

namespace EStore.Api.Endpoints.Stores.Queries;

public class GetAllStoresEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/stores", async ([AsParameters] GetAllStoresRequest requestParams, ISender sender) =>
        {
            var query = requestParams.Adapt<GetAllStoresQuery>();
            var result = await sender.Send(query);
            var response = result.Adapt<AppResponse<PaginatedResult<StoreDto>>>();
            return Results.Ok(response);
        })
        .WithName("GetAllStores")
        .Produces<AppResponse<PaginatedResult<StoreDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Get all stores with pagination.")
        .WithDescription("Retrieves a paginated list of stores.");
    }
} 