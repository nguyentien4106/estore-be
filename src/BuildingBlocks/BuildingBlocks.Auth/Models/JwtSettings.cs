namespace BuildingBlocks.Auth.Models;

public class JwtSettings
{
    public string SecretKey { get; set; } = default!;
    
    public string Issuer { get; set; } = default!;  
    
    public string Audience { get; set; } = default!;
    
    public string EStoreority { get; set; } = default!;
    
    public required int AccessTokenExpirationMinutes { get; set; }

    public required int RefreshTokenExpirationDays { get; set; }
    
    public required string RefreshTokenProvider { get; set; } = "Default";
}