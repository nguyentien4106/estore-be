using Estore.Application.Constants;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Files.Commands.UploadFile;

public record UploadFileCommand(IFormFile File, string UserName, int Width, int Height) : ICommand<AppResponse<FileInformationDto>>;

public class UploadFileValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileValidator()
    {
        RuleFor(x => x.File)
                    .NotNull().WithMessage("File is required.")
                    .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
                    .Must(file => file.Length <= FileConstants.FiveMBs).WithMessage($"File size must be less than {FileConstants.FiveMBs / FileConstants.OneMB}MB.");
        RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.")
                    .NotEmpty().WithMessage("UserName cannot be empty.");
        RuleFor(x => x.Width)
                    .NotNull().WithMessage("Width is required.")
                    .NotEmpty().WithMessage("Width cannot be empty.");
        RuleFor(x => x.Height)
                    .NotNull().WithMessage("Height is required.")
                    .NotEmpty().WithMessage("Height cannot be empty.");
    }
}
