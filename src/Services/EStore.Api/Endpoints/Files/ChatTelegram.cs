using BuildingBlocks.Models;
using Carter;
using Estore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files;

public class ChatTelegram : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/store/telegram", async ([FromServices]ITelegramService services , ISender sender) =>
        {
            var result = await services.GetChatIdAsync();

            return Results.Ok(result);
        })
        .Produces<long>(StatusCodes.Status201Created)
        .WithName("ChatTelegram")
        .WithTags("ChatTelegram")
        ;
    }
}
