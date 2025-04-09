using EStore.Application.Data;
using EStore.Domain.Enums;
using EStore.Domain.Models;

namespace EStore.Application.Auth.Commands.Auth.Register;

public class RegisterAccountHandler(IEStoreDbContext dbContext, UserManager<User> userManager) : ICommandHandler<RegisterAccountCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(RegisterAccountCommand command, CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            Email = command.Email,
            UserName = command.UserName.Split("@").First(),
            PhoneNumber = command.PhoneNumber,
            FirstName = command.FirstName,
            LastName = command.LastName,
        };

        var result = await userManager.CreateAsync(newUser, command.Password);
        if (result.Succeeded)
        {
            newUser.Status = (int)AccountStatus.Active;
            return AppResponse<bool>.Success(true);
        }

        return AppResponse<bool>.Error(result.Errors.FirstOrDefault()?.Description ?? "");

    }
}