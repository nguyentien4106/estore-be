namespace EStore.Application.Commands.Auth.Register;

public record RegisterAccountRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string UserName,
    string PhoneNumber);
