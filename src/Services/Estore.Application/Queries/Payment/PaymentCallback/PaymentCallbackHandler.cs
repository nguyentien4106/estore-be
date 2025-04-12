using BuildingBlocks.Auth.Models;
using EStore.Application.Services.Payment;
using EStore.Domain.Enums;
using VNPAY.NET.Models;

namespace EStore.Application.Queries.Payment.PaymentCallback;

public class PaymentCallbackHandler(IVnPayService vnPayService, IEStoreDbContext context) : IQueryHandler<PaymentCallbackQuery, AppResponse<PaymentResult>>
{
    public async Task<AppResponse<PaymentResult>> Handle(PaymentCallbackQuery request, CancellationToken cancellationToken)
    {
        var result = vnPayService.GetPaymentResult(request.query);

        var payment = await context.Payments.FindAsync(result.PaymentId, cancellationToken);
        if (payment == null)
        {
            return  AppResponse<PaymentResult>.NotFound("Payment", result.PaymentId);
        }
        
        UpdatePayment(payment, result);
        context.Payments.Update(payment);
        if (result.IsSuccess)
        {
            var account = await context.Users.FindAsync(payment.UserId, cancellationToken);

            account.AccountType = payment.OrderType == "UPGRADE_PRO" ? AccountType.Pro : AccountType.Plus;
            context.Users.Update(account);
        }

        await context.CommitAsync(cancellationToken);
        
        return result.IsSuccess ? AppResponse<PaymentResult>.Success(result) : AppResponse<PaymentResult>.Error(result.TransactionStatus.Description);
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
    }
}