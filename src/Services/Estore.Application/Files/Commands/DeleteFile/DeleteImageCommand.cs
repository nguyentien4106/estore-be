using Estore.Application.Dtos.Files;

namespace Estore.Application.Files.Commands.DeleteFile;

public record DeleteFileCommand(Guid Id) : ICommand<AppResponse<FileInformationDto>>;

public class DeleteFileValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileValidator(){
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.")
                    .NotEmpty().WithMessage("Id cannot be empty.");
    }
}
