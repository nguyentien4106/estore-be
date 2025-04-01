using Estore.Application.Constants;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Files.Commands.UploadFile;

public record UploadFileCommand(IFormFile File, string UserName) : ICommand<AppResponse<R2File>>;

public class UploadFileValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileValidator()
    {
        RuleFor(x => x.File)
                    .NotNull().WithMessage("File is required.")
                    .Must(file => file.ContentType == "image/*").WithMessage("File must be an image.")
                    .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
                    .Must(file => file.Length <= FileConstants.FiveMBs).WithMessage($"File size must be less than {FileConstants.FiveMBs / FileConstants.OneMB}MB.");
        RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.")
                    .NotEmpty().WithMessage("UserName cannot be empty.");
    }
}
