namespace EStore.Application.Commands.Files.DownloadFile;

public class DownloadFileR2Validator : AbstractValidator<DownloadFileR2Command>
{
    public DownloadFileR2Validator()
    {
        RuleFor(c => c.Id).NotNull().NotEmpty().WithMessage("Id is required.");
    }
}

public class DownloadFileTelegramValidator : AbstractValidator<DownloadFileTelegramCommand>
{
    public DownloadFileTelegramValidator()
    {
        RuleFor(c => c.Id).NotNull().NotEmpty().WithMessage("Id is required.");
    }
}