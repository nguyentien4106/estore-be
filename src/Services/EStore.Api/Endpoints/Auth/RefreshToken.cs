using BuildingBlocks.Models;
using Carter;
using EStore.Application.Commands.Auth.RefreshToken;
using Estore.Application.Models.Dtos;
using Mapster;

namespace EStore.Api.Endpoints.Auth;

public class RefreshToken : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh-token", async (RefreshTokenRequest request, ISender sender) =>
        {
            var command = request.Adapt<RefreshTokenCommand>();
            var result = await sender.Send(command);

            return Results.Ok(result);
        }).WithName("refresh-token")
        .Produces<AppResponse<AuthToken>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithDescription("Refresh access token using refresh token")
        .WithSummary("Refresh tokens");
    }
    
}