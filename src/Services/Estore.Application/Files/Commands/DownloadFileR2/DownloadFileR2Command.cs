namespace EStore.Application.Files.Commands.DownloadFileR2;
public record DownloadFileR2Command(Guid Id, StorageSource StorageSource) : ICommand<AppResponse<DownloadFileResponse>>;

public class DownloadFileValidator : AbstractValidator<DownloadFileR2Command>
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
