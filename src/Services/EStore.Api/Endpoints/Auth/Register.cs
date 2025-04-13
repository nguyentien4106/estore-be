using Carter;
using Mapster;
using BuildingBlocks.Models;
using EStore.Application.Commands.Auth.Register;

namespace EStore.Api.Endpoints.Auth;

public class Register : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterAccountRequest request, ISender sender) =>
        {
            var command = request.Adapt<RegisterAccountCommand>();
            var result = await sender.Send(command);
            
            return Results.Ok(result);
        }) .WithName("RegisterAccount")
        .Produces<AppResponse<bool>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Register Account")
        .WithDescription("Register Account");
    }
}