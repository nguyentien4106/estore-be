namespace Estore.Application.Files.Commands.DownloadFileTelegram;
public record DownloadFileTelegramCommand(Guid Id) : ICommand<AppResponse<DownloadFileResponse>>;

public class DownloadFileTelegramValidator : AbstractValidator<DownloadFileTelegramCommand>
{
    public DownloadFileTelegramValidator(){
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.")
                    .NotEmpty().WithMessage("Id cannot be empty.");
    }
}
