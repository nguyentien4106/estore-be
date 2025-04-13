using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Payment.Queries;

public class PaymentSuccess : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/payment/success", ([FromQuery] decimal amount, [FromQuery] string orderInfo, [FromQuery] string transactionNo, [FromQuery] string bankCode, [FromQuery] string payDate) =>
        {
            return Results.Ok(new
            {
                Status = "Success",
                Message = "Payment successful",
                Data = new
                {
                    Amount = amount,
                    OrderInfo = orderInfo,
                    TransactionNo = transactionNo,
                    BankCode = bankCode,
                    PayDate = payDate
                }
            });
        })
        .WithName("PaymentSuccess")
        .WithTags("Payment");
    }
} 