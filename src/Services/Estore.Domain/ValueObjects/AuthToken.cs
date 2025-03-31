namespace EStore.Domain.ValueObjects;

public class AuthToken
{
    public string AccessToken { get; set; }
    
    public string RefreshToken { get; set; }
}