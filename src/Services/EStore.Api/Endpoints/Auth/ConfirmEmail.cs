using Carter;
using EStore.Application.Commands.Auth.ConfirmEmail;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EStore.Api.Endpoints.Auth;


public class ConfirmEmail : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/confirm-email", async ([FromBody] ConfirmEmailRequest request, ISender sender) =>
        {
            var command = request.Adapt<ConfirmEmailCommand>();
            
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