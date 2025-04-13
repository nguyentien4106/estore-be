using Microsoft.AspNetCore.Mvc;
using VNPAY.NET;
namespace EStore.Api.Endpoints.Payment.Queries;

public class PaymentReturn : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/payment/return", async ( HttpContext context) =>
        {
            Console.WriteLine("PaymentReturn");
            if (context.Request.QueryString.HasValue)
            {
                
            }

            return Results.Ok("");
        })
        .WithName("PaymentReturn")
        .WithTags("Payment");
    }
} 