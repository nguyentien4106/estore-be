using EStore.Application.Data;
using EStore.Application.Enums;
using EStore.Domain.Models;

namespace EStore.Application.Auth.Commands.Auth.Register;

public class RegisterAccountHandler(IEStoreDbContext dbContext, UserManager<User> userManager) : ICommandHandler<RegisterAccountCommand, RegisterAccountResult>
{
    public async Task<RegisterAccountResult> Handle(RegisterAccountCommand command, CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            Email = command.Email,
            UserName = command.UserName,
            PhoneNumber = command.PhoneNumber,
            FirstName = command.FirstName,
            LastName = command.LastName,
            
        };
        var result = await userManager.CreateAsync(newUser, command.Password);
        if (result.Succeeded)
        {
            newUser.Status = (int)AccountStatus.Active;
        }
        
        return new RegisterAccountResult(result);
    }
}