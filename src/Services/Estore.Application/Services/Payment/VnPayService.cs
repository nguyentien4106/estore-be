using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using EStore.Application.Commands.Payment.CreatePayment;
using Microsoft.AspNetCore.Http;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace EStore.Application.Services.Payment;

public class VNPayService : IVnPayService
{
    private readonly VNPayConfiguration _vnPayConfiguration;
    private readonly IVnpay _vnpay;
    
    public VNPayService(VNPayConfiguration vnPayConfig)
    {
        _vnPayConfiguration = vnPayConfig;
        _vnpay = new Vnpay();
        _vnpay.Initialize(_vnPayConfiguration.TmnCode, _vnPayConfiguration.HashSecret, _vnPayConfiguration.BaseUrl, _vnPayConfiguration.ReturnUrl);
    }

    public string CreatePaymentUrl(CreatePaymentCommand request)
    {
        try
        {
            var value = new PaymentRequest
            {
                PaymentId = request.PaymentId,
                Money = request.Amount,
                Description = request.OrderInfo,
                IpAddress = request.IpAddress,
                BankCode = BankCode.ANY,
                CreatedDate = DateTime.Now,
                Currency = Currency.VND,
                Language = DisplayLanguage.English,
            };

            return _vnpay.GetPaymentUrl(value);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public PaymentResult GetPaymentResult(IQueryCollection query)
    {
        return _vnpay.GetPaymentResult(query);
    }
}
