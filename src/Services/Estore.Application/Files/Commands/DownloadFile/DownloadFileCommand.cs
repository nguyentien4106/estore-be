namespace Estore.Application.Files.Commands.DownloadFile;
public record DownloadFileCommand(Guid Id, StorageSource StorageSource) : ICommand<AppResponse<DownloadFileResponse>>;

public class DownloadFileValidator : AbstractValidator<DownloadFileCommand>
{
    public DownloadFileValidator(){
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.")
                    .NotEmpty().WithMessage("Id cannot be empty.");

        RuleFor(x => x.StorageSource)
                    .NotNull().WithMessage("StorageSource is required.")
                    .NotEmpty().WithMessage("StorageSource cannot be empty.");
    }
}
