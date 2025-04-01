using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EStore.Domain.Models;
using BuildingBlocks.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EStore.Application.Helpers;

public static class TokenUtils
{
    public static async Task<string> GenerateAccessToken(UserManager<User> userManager, JwtSettings appSettings, User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var userRoles = await userManager.GetRolesAsync(user);

        // Add role claims
        var claims = userRoles.Select(role => new Claim(ClaimTypes.Role, role));
        
        var userClaims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new ("userName", user.UserName ?? ""),
            new("email", user.Email ?? "")
        };
        
        userClaims.AddRange(claims);
        
        var tokenOptions = new JwtSecurityToken(
            issuer: appSettings.Issuer,
            audience: appSettings.Audience,
            claims: userClaims,
            expires: DateTime.UtcNow.AddSeconds(appSettings.AccessTokenExpirationMinutes),
            signingCredentials: signInCredentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public static async Task<string> GenerateRefreshToken(UserManager<User> userManager, JwtSettings jwtSettings, User user)
    {
        const string refreshTokenKey = "RefreshToken";
        var refreshToken = await userManager.GenerateUserTokenAsync(user, jwtSettings.RefreshTokenProvider, refreshTokenKey);
        return refreshToken;
    }
    
    public static ClaimsPrincipal GetPrincipalFromExpiredToken(JwtSettings appSettings, string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = appSettings.Audience,
            ValidIssuer = appSettings.Issuer,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey))
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("GetPrincipalFromExpiredToken Token is not validated");

        return principal;
    }

}