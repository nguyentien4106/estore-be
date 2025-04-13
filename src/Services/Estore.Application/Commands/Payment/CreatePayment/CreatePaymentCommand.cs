using BuildingBlocks.CQRS;
using EStore.Domain.Enums;

namespace EStore.Application.Commands.Payment.CreatePayment;

public record CreatePaymentCommand(
    double Amount,
    string OrderInfo,
    string OrderType,
    string BankCode,
    string Language,
    string UserId,
    SubscriptionType SubscriptionType,
    string IpAddress,
    long PaymentId
    ) : ICommand<AppResponse<string>>; 