using BuildingBlocks.Models; // For AppResponse
using Carter;
using EStore.Application.Data;
using EStore.Application.Queries.Stores.GetAllStores;
using EStore.Application.Services.Telegram;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EStore.Api.Endpoints.Stores.Commands;

public class DeleteStoreEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/stores/{id}", async (Guid id, [FromServices] IEStoreDbContext dbContext, [FromServices] ITelegramService telegramService, ISender sender) =>
        {
            var store = await dbContext.Stores.FindAsync(id);
            if (store == null)
            {
                return Results.NotFound();
            }

            var result = await telegramService.DeleteChannelAsync(store.ChannelId, store.AccessHash);
            if (result.Succeed)
            {
                dbContext.Stores.Remove(store);
                await dbContext.CommitAsync();
                return Results.Ok(result);
            }
            else
            {
                return Results.BadRequest(result);
            }
        })
        .WithName("DeleteStore")
        .WithTags("Stores")
        .WithSummary("Delete a store.")
        .WithDescription("Deletes a store with the provided Id.");
    }
} 