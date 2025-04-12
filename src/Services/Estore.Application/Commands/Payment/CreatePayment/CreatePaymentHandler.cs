using Estore.Domain.Enums;
using EStore.Application.Services.Payment;
using EStore.Domain.Enums;

namespace EStore.Application.Commands.Payment.CreatePayment;

public class CreatePaymentHandler(IVnPayService vnPayService, IEStoreDbContext context) : ICommandHandler<CreatePaymentCommand, AppResponse<string>>
{
    public async Task<AppResponse<string>> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = new Order
            {
                PaymentId = command.PaymentId,
                UserId = command.UserId,
                Amount = command.Amount,
                OrderType = command.OrderType,
                OrderCode = command.OrderType + "-" + DateTime.Now.Ticks.ToString(),
                Status = OrderStatus.Pending,
                Description = command.OrderInfo,
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now,
            };
            await context.Orders.AddAsync(order, cancellationToken);
            var payment = new Domain.Models.Payment
            {
                Id = command.PaymentId,
                UserId = command.UserId,
                Amount = command.Amount,
                OrderId = order.Id,
                Description = command.OrderInfo,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now,
                VnpIpAddress = command.IpAddress,
                OrderType = command.OrderType,
            };
            await context.Payments.AddAsync(payment, cancellationToken);
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
} 