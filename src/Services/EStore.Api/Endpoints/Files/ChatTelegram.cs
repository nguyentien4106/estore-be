using BuildingBlocks.Models;
using Carter;
using EStore.Application.Services;
using EStore.Application.Services.Telegram;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Files;

public class ChatTelegram : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/telegram", async ([FromServices]ITelegramService services , ISender sender) =>
        {
            return Results.Ok(1);
        })
        .Produces<long>(StatusCodes.Status201Created)
        .WithName("ChatTelegram")
        .WithTags("ChatTelegram")
        ;
    }
}
