namespace EStore.Application.Commands.Auth.ChangePassword;

public class ChangePasswordHandler(IEStoreDbContext context) : ICommandHandler<ChangePasswordCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName == command.UserName, cancellationToken);

        if (user == null)
        {
            return AppResponse<bool>.NotFound("User", command.UserName);
        }

        if (!BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
        {
            return AppResponse<bool>.Error("Current password is incorrect");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        await context.CommitAsync(cancellationToken);

        return AppResponse<bool>.Success(true);
    }
} 