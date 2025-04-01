
namespace Estore.Application.Files.Commands.DeleteFile;
public record DeleteFileCommand(string FileName) : ICommand<AppResponse<R2File>>;

public class DeleteFileValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileValidator(){
        RuleFor(x => x.FileName)
                    .NotNull().WithMessage("UserName is required.")
                    .NotEmpty().WithMessage("UserName cannot be empty.");
    }
}
