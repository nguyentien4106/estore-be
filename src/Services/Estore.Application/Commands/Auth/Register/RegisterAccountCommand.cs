using BuildingBlocks.CQRS;
using FluentValidation;

namespace EStore.Application.Commands.Auth.Register;

public record RegisterAccountCommand(string FirstName, string LastName, string Email, string Password, string UserName, string PhoneNumber) : ICommand<AppResponse<bool>>;