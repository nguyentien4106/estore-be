using EStore.Domain.Models.Base;

namespace EStore.Application.Queries.Files.GetR2FilePreview;

public class GetR2FilePreviewValidator : AbstractValidator<GetR2FilePreviewQuery>
{
    public GetR2FilePreviewValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("File Id is required.");
    }
} 