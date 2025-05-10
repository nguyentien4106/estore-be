using BuildingBlocks.Auth.Models;
using EStore.Application.Constants;
using EStore.Application.Services.Payment;
using EStore.Domain.Enums;
using VNPAY.NET.Models;

namespace EStore.Application.Queries.Payment.PaymentCallback;

public class PaymentCallbackHandler(IVnPayService vnPayService, IEStoreDbContext context) : IQueryHandler<PaymentCallbackQuery, AppResponse<PaymentResult>>
{
    public async Task<AppResponse<PaymentResult>> Handle(PaymentCallbackQuery request, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = vnPayService.GetPaymentResult(request.query);
            var payment = await GetPaymentAsync(result.PaymentId, cancellationToken);
            if (payment == null)
            {
                return AppResponse<PaymentResult>.NotFound("Payment", result.PaymentId);
            }

            UpdatePayment(payment, result);

            if (result.IsSuccess)
            {
                var user = await GetUserAsync(payment.UserId, cancellationToken);
                if (user == null)
                {
                    return AppResponse<PaymentResult>.NotFound("User", payment.UserId);
                }

                await ProcessSuccessfulPaymentAsync(user, payment, cancellationToken);
            }

            await context.CommitAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result.IsSuccess 
                ? AppResponse<PaymentResult>.Success(result) 
                : AppResponse<PaymentResult>.Error(result.TransactionStatus.Description);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return AppResponse<PaymentResult>.Error(ex.Message);
        }
    }

    private async Task<Domain.Models.Payment?> GetPaymentAsync(long paymentId, CancellationToken cancellationToken)
    {
        return await context.Payments.FindAsync(new object[] { paymentId }, cancellationToken);
    }

    private async Task<User?> GetUserAsync(string userId, CancellationToken cancellationToken)
    {
        return await context.Users.FindAsync(new object[] { userId }, cancellationToken);
    }

    private async Task ProcessSuccessfulPaymentAsync(User user, Domain.Models.Payment payment, CancellationToken cancellationToken)
    {
        var accountType = GetAccountType(payment.OrderType);
        await UpdateUserAccountTypeAsync(user, accountType);
        await CreateSubscriptionAsync(user, accountType, payment.SubscriptionType, cancellationToken);
    }

    private static AccountType GetAccountType(string orderType)
    {
        return orderType == PaymentConstants.UpgradePro ? AccountType.Pro : AccountType.Plus;
    }

    private Task UpdateUserAccountTypeAsync(User user, AccountType accountType)
    {
        user.AccountType = accountType;
        context.Users.Update(user);
        return Task.CompletedTask;
    }

    private async Task CreateSubscriptionAsync(User user, AccountType accountType, SubscriptionType subscriptionType, CancellationToken cancellationToken)
    {
        var subscription = new Subscription
        {
            UserId = user.Id,
            AccountType = accountType,
            IsActive = true,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths((int)subscriptionType),
            LastModified = DateTime.UtcNow,
        };
        
        await context.Subscriptions.AddAsync(subscription, cancellationToken);
    }

    private static void UpdatePayment(Domain.Models.Payment payment, PaymentResult result)
    {
        payment.Status = result.IsSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
        payment.Success = result.IsSuccess;
        payment.VnpPayDate = result.Timestamp;
        payment.VnpOrderInfo = result.Description;
        payment.VnpBankCode = result.BankingInfor.BankCode;
        payment.VnpBankTransactionId = result.BankingInfor.BankTransactionId;
        payment.VnpResponseCode = (int)result.PaymentResponse.Code;
        payment.VnpResponseDescription = result.PaymentResponse.Description;
        payment.VnpTransactionCode = (int)result.TransactionStatus.Code;
        payment.VnpTransactionDescription = result.TransactionStatus.Description;
        payment.VnpTransactionId = result.VnpayTransactionId;
        payment.VnpPaymentMethod = result.PaymentMethod;
        payment.LastModified = DateTime.UtcNow;
    }
}