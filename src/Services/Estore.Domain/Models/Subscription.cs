using BuildingBlocks.Auth.Models;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;

public class Subscription : Entity<Guid>
{
    public string UserId { get; set; } = default!;

    public AccountType AccountType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}