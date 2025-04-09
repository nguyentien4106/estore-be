using Microsoft.AspNetCore.Authorization;

namespace EStore.Api.Middlewares;

public class AccountRequirement(string accountType) : IAuthorizationRequirement
{
    public string AccountType { get; private set; } = accountType;

}