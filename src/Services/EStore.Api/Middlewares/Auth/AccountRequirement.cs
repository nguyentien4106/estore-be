using Microsoft.AspNetCore.Authorization;

namespace EStore.Api.Middlewares.Auth;

public class AccountRequirement(string accountType) : IAuthorizationRequirement
{
    public string AccountType { get; private set; } = accountType;

}