using BuildingBlocks.Auth.Models;
using EStore.Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace EStore.Domain.Models;

public class User : IdentityUser, IEntity
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public int Status { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; } = default!;

    public DateTime? LastModified { get; set; } = DateTime.UtcNow;

    public string? LastModifiedBy { get; set; } = default!;

    public AccountType AccountType { get; set; } = AccountType.Free;
}