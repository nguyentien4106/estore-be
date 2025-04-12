using BuildingBlocks.Models;
using Carter;
using EStore.Application.Commands.Auth.Login;
using Estore.Application.Models.Dtos;
using Mapster;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;

namespace EStore.Api.Endpoints.Auth;

public class Login : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (LoginRequest request, ISender sender) =>
        {
            var command = request.Adapt<LoginCommand>();
            var result = await sender.Send(command);

            return Results.Ok(result);
        }).WithName("LoginAccount")
        .Produces<AppResponse<AuthToken>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Login Account")
        .WithDescription("Login Account");
    }
}