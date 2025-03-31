namespace EStore.Application.Dtos.EStore;

public record RegisterAccountRequest(string FirstName, string LastName, string Email, string Password, string UserName, string PhoneNumber);

public record RegisterAccountResponse(IdentityResult Result);