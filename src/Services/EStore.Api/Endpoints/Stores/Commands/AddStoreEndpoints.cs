using BuildingBlocks.Models; // For AppResponse
using Carter;
using EStore.Application.Commands.Stores.AddStore;
using EStore.Application.Queries.Stores.GetAllStores;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EStore.Api.Endpoints.Stores.Commands;

public class AddStoreEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/stores", async (AddStoreRequest request, ISender sender) =>
        {
            var command = request.Adapt<AddStoreCommand>();
            
            var result = await sender.Send(command);

            return Results.Created($"/stores/{result.Data?.Id ?? Guid.Empty}", result);
        })
        .WithName("AddStore")
        .WithTags("Stores")
        .Produces<AppResponse<StoreDto>>(StatusCodes.Status201Created)
        .Produces<AppResponse<object>>(StatusCodes.Status400BadRequest) // For validation errors if not handled by ValidationProblem
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity) // Example for FluentValidation.Results.ValidationProblem 
        .WithSummary("Add a new store.")
        .WithDescription("Creates a new store with the provided Name and ChannelName.");
    }
} 