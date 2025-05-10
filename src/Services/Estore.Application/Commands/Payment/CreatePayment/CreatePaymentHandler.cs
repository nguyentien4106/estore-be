using EStore.Domain.Enums;
using EStore.Application.Services.Payment;

namespace EStore.Application.Commands.Payment.CreatePayment;

public class CreatePaymentHandler(IVnPayService vnPayService, IEStoreDbContext context) : ICommandHandler<CreatePaymentCommand, AppResponse<string>>
{
    public async Task<AppResponse<string>> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = await CreateOrderAsync(command, cancellationToken);
            var payment = await CreatePaymentAsync(command, order.Id, cancellationToken);
            
            await context.CommitAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            return AppResponse<string>.Success(vnPayService.CreatePaymentUrl(command));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AppResponse<string>.Error(ex.Message);
        }
    }

    private async Task<Order> CreateOrderAsync(CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            PaymentId = command.PaymentId,
            UserId = command.UserId,
            Amount = command.Amount,
            OrderType = command.OrderType,
            OrderCode = GenerateOrderCode(command.OrderType),
            Status = OrderStatus.Processing,
            Description = command.OrderInfo,
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            SubscriptionType = command.SubscriptionType,
        };
        
        await context.Orders.AddAsync(order, cancellationToken);
        return order;
    }

    private async Task<Domain.Models.Payment> CreatePaymentAsync(CreatePaymentCommand command, Guid orderId, CancellationToken cancellationToken)
    {
        var payment = new Domain.Models.Payment
        {
            Id = command.PaymentId,
            UserId = command.UserId,
            Amount = command.Amount,
            OrderId = orderId,
            Description = command.OrderInfo,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            VnpIpAddress = command.IpAddress,
            OrderType = command.OrderType,
            SubscriptionType = command.SubscriptionType,
        };
        
        await context.Payments.AddAsync(payment, cancellationToken);
        return payment;
    }

    private static string GenerateOrderCode(string orderType)
    {
        return $"{orderType}-{DateTime.UtcNow.Ticks}";
    }
} 