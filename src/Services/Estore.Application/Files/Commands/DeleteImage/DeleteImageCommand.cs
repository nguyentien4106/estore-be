
namespace Estore.Application.Files.Commands.DeleteImage;
public record DeleteImageCommand(string FileName) : ICommand<AppResponse<R2File>>;

public class DeleteImageValidator : AbstractValidator<DeleteImageCommand>
{
    public DeleteImageValidator(){
        RuleFor(x => x.FileName)
                    .NotNull().WithMessage("UserName is required.")
                    .NotEmpty().WithMessage("UserName cannot be empty.");
    }
}
