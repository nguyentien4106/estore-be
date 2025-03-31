using Estore.Application.Constants;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Store.Commands;

public record StoreImageCommand(IFormFile File) : ICommand<AppResponse<string>>;

public class StoreImageValidator : AbstractValidator<StoreImageCommand>
{
    public StoreImageValidator()
    {
        RuleFor(x => x.File)
                    .NotNull().WithMessage("File is required.")
                    .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
                    .Must(file => file.Length <= FileConstants.FiveMBs).WithMessage($"File size must be less than {FileConstants.FiveMBs / FileConstants.OneMB}MB.");
    }
}
