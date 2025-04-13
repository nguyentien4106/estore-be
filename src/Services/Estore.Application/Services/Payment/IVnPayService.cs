using EStore.Application.Commands.Payment.CreatePayment;
using Microsoft.AspNetCore.Http;
using VNPAY.NET.Models;

namespace EStore.Application.Services.Payment;

public interface IVnPayService
{
    string CreatePaymentUrl(CreatePaymentCommand command);
    
    PaymentResult GetPaymentResult(IQueryCollection query);
}
