namespace EStore.Application.Commands.Files.DeleteFile;

public class DeleteFileValidator : AbstractValidator<DeleteFileR2Command>
{
    public DeleteFileValidator(){
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.")
                    .NotEmpty().WithMessage("Id cannot be empty.");

    }
}
