using BuildingBlocks.Models;
using Carter;
using EStore.Application.Commands.Auth.ForgotPassword;
using Mapster;

namespace EStore.Api.Endpoints.Auth;

public class ForgotPassword : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/forgot-password", async (ForgotPasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<ForgotPasswordCommand>();
            var result = await sender.Send(command);
            
            return Results.Ok(result);
        })
            .WithName("forgot-password")
            .Produces<AppResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("forgot password");
    }
}