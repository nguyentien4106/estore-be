using Microsoft.AspNetCore.Http;
using VNPAY.NET.Models;

namespace EStore.Application.Queries.Payment.PaymentCallback;

public record PaymentCallbackQuery(IQueryCollection query) : IQuery<AppResponse<PaymentResult>>;