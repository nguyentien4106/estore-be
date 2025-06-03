using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using EStore.Services.Estore.Application.Commands.Stores.AddStore;

namespace EStore.Api.Endpoints.Stores.Commands;

public class AddStoreEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/stores", async (AddStoreRequest request, ISender sender) =>
        {
            var command = new AddStoreCommand(request.ChannelName);
            var result = await sender.Send(command);
            return Results.Created($"/stores/{result.StoreId}", result); // Assuming StoreId is part of the response for the location header
        })
        .WithName("AddStore")
        .Produces<AddStoreResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Add a new store.")
        .WithDescription("Creates a new store with the provided channel name.");
    }
} 