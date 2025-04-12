using EStore.Application.Constants;

namespace EStore.Application.Commands.Files.UploadFile;

public class UploadFileR2Validator : AbstractValidator<UploadFileR2Command>
{
    public UploadFileR2Validator()
    {
        RuleFor(c => c.UserName).NotEmpty().WithMessage("Username is required");
        RuleFor(c => c.File).NotNull().WithMessage("File is required");
    }
}

public class UploadFileTelegramValidator : AbstractValidator<UploadFileTelegramCommand>
{
    public UploadFileTelegramValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.")
            .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
            .Must(file => file.Length <= FileSizeLimits.FiveMBs).WithMessage($"File size must be less than {FileSizeLimits.FiveMBs / FileSizeLimits.OneMB}MB.");
        
        RuleFor(x => x.UserName)
            .NotNull().WithMessage("UserName is required.")
            .NotEmpty().WithMessage("UserName cannot be empty.");
    }
}
