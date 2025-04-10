using EStore.Application.Constants;
using EStore.Domain.Models.Base;
using Microsoft.AspNetCore.Http;

namespace EStore.Application.Files.Commands.UploadFileTelegram;

public record UploadFileTelegramCommand(IFormFile File, string UserName) : ICommand<AppResponse<FileEntityResponse>>;

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
