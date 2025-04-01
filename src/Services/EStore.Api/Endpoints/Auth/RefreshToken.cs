using EStore.Application.Auth.Commands.Auth.RefreshToken;
using BuildingBlocks.Models;
using Carter;
using Mapster;

namespace EStore.Api.Endpoints;

public class RefreshToken : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh-token", async (string refreshToken, ISender sender) =>
        {
            var command = new RefreshTokenCommand(refreshToken);
            var result = await sender.Send(command);

            return Results.Ok(result);
        }).WithName("refresh-token")
        .Produces<AppResponse<AuthToken>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithDescription("Refresh access token using refresh token")
        .WithSummary("Refresh tokens");
    }
    
}