using Estore.Application.Constants;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Files.Commands.UploadFileTelegram;

public record UploadFileTelegramCommand(IFormFile File, string UserName, int Width, int Height) : ICommand<AppResponse<TeleFileLocation>>;

public class UploadFileTelegramValidator : AbstractValidator<UploadFileTelegramCommand>
{
    public UploadFileTelegramValidator()
    {
        RuleFor(x => x.File)
                    .NotNull().WithMessage("File is required.")
                    .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
                    .Must(file => file.Length <= FileConstants.FiveMBs).WithMessage($"File size must be less than {FileConstants.FiveMBs / FileConstants.OneMB}MB.");
        
        RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.")
                    .NotEmpty().WithMessage("UserName cannot be empty.");
    }
}
