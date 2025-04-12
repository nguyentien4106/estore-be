namespace EStore.Application.Commands.Payment.CreatePayment;

public record CreatePaymentRequest
{
    public double Amount { get; init; }
    public string OrderInfo { get; init; }
    public string OrderType { get; init; }
    public string BankCode { get; init; } = "";
    public string Language { get; init; } = "en";
    public string UserId { get; init; }
} 