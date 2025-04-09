namespace EStore.Application.Files.Commands.DownloadFileR2;
public record DownloadFileR2Command(Guid Id) : ICommand<AppResponse<DownloadFileResponse>>;

public class DownloadFileValidator : AbstractValidator<DownloadFileR2Command>
{
    public DownloadFileValidator(){
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.")
                    .NotEmpty().WithMessage("Id cannot be empty.");

    }
}
