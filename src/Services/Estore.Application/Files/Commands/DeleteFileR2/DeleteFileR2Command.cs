using EStore.Application.Dtos.Files;

namespace EStore.Application.Files.Commands.DeleteFileR2;

public record DeleteFileR2Command(Guid Id) : ICommand<AppResponse<Guid>>;

public class DeleteFileValidator : AbstractValidator<DeleteFileR2Command>
{
    public DeleteFileValidator(){
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.")
                    .NotEmpty().WithMessage("Id cannot be empty.");

    }
}
