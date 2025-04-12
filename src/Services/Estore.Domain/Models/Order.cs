using Estore.Domain.Enums;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;
public class Order : Entity<Guid>
{
    public string UserId {get;set;}
    public long PaymentId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string OrderType {get;set;} = string.Empty;
    public string Description { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
