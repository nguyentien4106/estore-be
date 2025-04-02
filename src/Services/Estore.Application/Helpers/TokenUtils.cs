using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EStore.Domain.Models;
using BuildingBlocks.Auth.Models;
using Microsoft.IdentityModel.Tokens;
using EStore.Application.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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
            expires: DateTime.Now.AddMinutes(appSettings.AccessTokenExpirationMinutes),
            signingCredentials: signInCredentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public static async Task<string> GenerateRefreshToken(JwtSettings jwtSettings, User user, IEStoreDbContext context)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.Now.AddDays(jwtSettings.RefreshTokenExpirationDays);
        await context.CommitAsync();

        return refreshToken;
    }

    private static string GenerateRefreshToken(int size = 64)
    {
        var randomNumber = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    public static async Task<User?> GetUserFromRefreshToken(IEStoreDbContext context, string refreshToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public static bool IsRefreshTokenValid(DateTime? refreshTokenExpiry)
    {
        return refreshTokenExpiry >= DateTime.Now;
    }
}