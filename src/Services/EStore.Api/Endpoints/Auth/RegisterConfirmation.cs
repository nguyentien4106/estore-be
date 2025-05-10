using Carter;
using EStore.Application.Commands.Auth.ConfirmEmail;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EStore.Api.Endpoints.Auth;

public class RegisterConfirmation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/confirm-email", async (string userId, string token, ISender sender) =>
        {
            var command = new ConfirmEmailCommand 
            { 
                UserId = userId, 
                Token = token 
            };
            
            var result = await sender.Send(command);
            
            return Results.Ok(result);
        })
        .WithName("ConfirmEmail")
        .WithTags("Auth")
        .Produces<AppResponse<bool>>(StatusCodes.Status200OK)
        .Produces<AppResponse<bool>>(StatusCodes.Status400BadRequest)
        .WithDescription("Confirms a user\'s email address using a token.")
        .WithSummary("Confirm user email");
    }
}