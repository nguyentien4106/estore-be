using BuildingBlocks.Pagination;
using EStore.Application.Queries.Stores.GetAllStores;

using EStore.Application.Services.Telegram;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Stores.Queries;

public class TestEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/test",async ([FromServices]ITelegramService telegramService, ISender sender) =>
        {
            var result = await telegramService.TestAsync();
            return Results.Ok(result);
        })
        .WithName("test")
        .Produces<AppResponse<PaginatedResult<StoreDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Get all stores with pagination.")
        .WithDescription("Retrieves a paginated list of stores.");
    }
} 