namespace EStore.Application.Commands.Files.DeleteFile;

public class DeleteFileR2Validator : AbstractValidator<DeleteFileR2Command>
{
    public DeleteFileR2Validator()
    {
        RuleFor(c => c.Id).NotNull().NotEmpty().WithMessage("Id is required.");
    }
}

public class DeleteFileTelegramValidator : AbstractValidator<DeleteFileTelegramCommand>
{
    public DeleteFileTelegramValidator()
    {
        RuleFor(c => c.Id).NotNull().NotEmpty().WithMessage("Id is required.");
    }
}