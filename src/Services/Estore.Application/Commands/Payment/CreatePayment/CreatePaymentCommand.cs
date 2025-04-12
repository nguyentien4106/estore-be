using BuildingBlocks.CQRS;

namespace EStore.Application.Commands.Payment.CreatePayment;

public record CreatePaymentCommand(
    double Amount,
    string OrderInfo,
    string OrderType,
    string BankCode,
    string Language,
    string UserId,
    string IpAddress,
    long PaymentId) : ICommand<AppResponse<string>>; 