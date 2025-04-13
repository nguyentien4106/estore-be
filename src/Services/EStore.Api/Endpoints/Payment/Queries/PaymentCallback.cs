using EStore.Application.Queries.Payment.PaymentCallback;
namespace EStore.Api.Endpoints.Payment.Queries;

public class PaymentCallback : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/payment/callback", async (HttpContext context, ISender sender) =>
        {
            if (context.Request.QueryString.HasValue)
            {
                var query = new PaymentCallbackQuery(context.Request.Query);
                
                var result = await sender.Send(query);
                
                return Results.Ok(result);
            }

            return Results.Ok("");
        })
        .WithName("PaymentCallback")
        .WithTags("Payment");
    }
} 