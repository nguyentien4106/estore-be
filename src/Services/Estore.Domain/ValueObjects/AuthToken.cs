namespace EStore.Domain.ValueObjects;

public class EStoreToken
{
    public string AccessToken { get; set; }
    
    public string RefreshToken { get; set; }
}