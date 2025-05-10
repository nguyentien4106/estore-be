using BuildingBlocks.Auth.Models;
using EStore.Domain.Enums;

namespace EStore.Application.Models.Dashboard;

public class UserSubscriptionDto
{
    public AccountType AccountType { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public bool IsExpired => DateTime.UtcNow > EndDate;
} 