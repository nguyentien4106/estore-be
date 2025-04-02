using Estore.Application.Dtos.Files;

namespace Estore.Application.Files.Commands.DeleteFileTelegram;

public record DeleteFileTelegramCommand(Guid Id) : ICommand<AppResponse<FileInformationDto>>;

public class DeleteFileTelegramValidator : AbstractValidator<DeleteFileTelegramCommand>
{
    public DeleteFileTelegramValidator(){
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.")
                    .NotEmpty().WithMessage("Id cannot be empty.");
    }
}
