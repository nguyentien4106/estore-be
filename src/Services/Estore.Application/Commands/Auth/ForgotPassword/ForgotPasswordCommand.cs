namespace EStore.Application.Commands.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand<AppResponse<bool>>;
