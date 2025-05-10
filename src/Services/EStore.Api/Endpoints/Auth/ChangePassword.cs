using EStore.Application.Commands.Auth.ChangePassword;
using Carter;

namespace EStore.Api.Endpoints.Auth;

public class ChangePassword : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/change-password", async (ChangePasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<ChangePasswordCommand>();

            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .WithName("ChangePassword")
        .WithTags("Auth")
        .RequireAuthorization()
        .Produces<AppResponse<bool>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("Change user password")
        .WithSummary("Change password endpoint");
    }
} 