using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Payment.Queries;

public class PaymentFailed : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/payment/failed", ([FromQuery] decimal amount, [FromQuery] string orderInfo, [FromQuery] string responseCode) =>
        {
            return Results.Ok(new
            {
                Status = "Failed",
                Message = "Payment failed",
                Data = new
                {
                    Amount = amount,
                    OrderInfo = orderInfo,
                    ResponseCode = responseCode
                }
            });
        })
        .WithName("PaymentFailed")
        .WithTags("Payment");
    }
} 