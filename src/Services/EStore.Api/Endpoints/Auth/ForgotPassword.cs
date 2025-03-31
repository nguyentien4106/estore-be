using EStore.Application.Auth.Commands.Auth.ForgotPassword;
using BuildingBlocks.Models;
using Carter;

namespace EStore.Api.Endpoints;

public class ForgotPassword : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/EStore/forgot-password", async (string email, ISender sender) =>
        {
            var command = new ForgotPasswordCommand(email);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
            .WithName("forgot-password")
            .Produces<AppResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("forgot password");
    }
}