using EStore.Application.Auth.Commands.Auth.RefreshToken;
using BuildingBlocks.Models;
using Carter;
using Mapster;

namespace EStore.Api.Endpoints;

public class RefreshToken : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/EStore/refresh-token", async (EStoreToken request, ISender sender) =>
        {
            var command = request.Adapt<RefreshTokenCommand>();
            var result = await sender.Send(command);

            return Results.Ok(result);
        }).WithName("refresh-token")
        .Produces<AppResponse<EStoreToken>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithDescription("refresh a token")
        .WithSummary("refresh a token");
    }
    
}