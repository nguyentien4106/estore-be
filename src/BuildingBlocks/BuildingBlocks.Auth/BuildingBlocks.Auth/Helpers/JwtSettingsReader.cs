using System;
using BuildingBlocks.Auth.Models;
using Microsoft.Extensions.Configuration;

public class JwtSettingsReader
{
    private readonly JwtSettings _jwtSettings;
    
    public JwtSettingsReader()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Sets the base path
            .AddJsonFile("jwtsettings.json", optional: false, reloadOnChange: true); // Load the JSON file

        var configuration = builder.Build();
        
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new InvalidOperationException("JWTSettings not found");
    }

    public JwtSettings JwtSettings => _jwtSettings;

    public static JwtSettings GetJwtSettings()
    {
        var configuration = new JwtSettingsReader();
        return configuration.JwtSettings;
    }
    
}