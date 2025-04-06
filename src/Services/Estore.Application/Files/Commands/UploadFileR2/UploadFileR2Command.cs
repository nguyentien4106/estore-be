using EStore.Domain.Models.Base;
using Microsoft.AspNetCore.Http;

namespace EStore.Application.Files.Commands.UploadFile;

public record UploadFileR2Command(IFormFile File, string UserName) : ICommand<AppResponse<FileEntity>>;

public class UploadFileValidator : AbstractValidator<UploadFileR2Command>
{
    public UploadFileValidator()
    {
        RuleFor(x => x.File)
                    .NotNull().WithMessage("File is required.")
                    .Must(file => file.Length > 0).WithMessage("File cannot be empty.");
        RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.")
                    .NotEmpty().WithMessage("UserName cannot be empty.");
    }
}
