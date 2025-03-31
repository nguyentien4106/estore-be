using System.Text;
using BuildingBlocks.Auth.Models;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Auth.Constants;

public static class Constants
{
    public static TokenValidationParameters GetTokenValidationParameters(JwtSettings jwtSettings)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromSeconds(0)
        };
        
    }
}

public static class ClaimNames
{
    public static string UserName = "userName";
}