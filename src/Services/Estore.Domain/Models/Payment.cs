using EStore.Domain.Abstractions;
using EStore.Domain.Enums;

namespace EStore.Domain.Models;

public class Payment : Entity<long>
{
    public string UserId { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public double Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod Method { get; set; } = PaymentMethod.VNPay;
    public string OrderType { get; set; } = string.Empty;
    public bool Success { get; set; } = false;
    
    // VNPay Payment Information
    public string? VnpIpAddress { get; set; }
    public int? VnpTransactionCode { get; set; }
    public string? VnpBankCode { get; set; }
    public DateTime? VnpPayDate { get; set; }
    public int? VnpResponseCode { get; set; }
    public string? VnpResponseDescription { get; set; }
    public string? VnpSecureHash { get; set; }
    public string? VnpTxnRef { get; set; }
    public string? VnpOrderInfo { get; set; }
    public string? VnpBankTransactionId { get; set; }
    public string? VnpTransactionDescription { get; set; }
    public long? VnpTransactionId { get; set; }
    public string? VnpPaymentMethod { get; set; }

}
