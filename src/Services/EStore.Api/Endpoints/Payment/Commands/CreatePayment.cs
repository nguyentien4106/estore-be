using EStore.Application.Commands.Payment.CreatePayment;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Payment.Commands;

public class CreatePayment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/payment/create", async ([FromBody] CreatePaymentRequest request, ISender sender, HttpContext httpContext) =>
        {
            var command = new CreatePaymentCommand(
                request.Amount,
                request.OrderInfo,
                request.OrderType,
                request.BankCode,
                request.Language,
                request.UserId,
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                DateTime.Now.Ticks
            );

            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .Produces<AppResponse<string>>(StatusCodes.Status200OK)
        .WithName("CreatePayment")
        .WithTags("Payment");
    }
} 