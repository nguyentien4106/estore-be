using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace EStore.Application.Queries.Stores.GetAllStores;

public class GetAllStoresQueryValidator : AbstractValidator<GetAllStoresQuery>
{
    private readonly List<string> _allowedSortFields = new List<string> { "Id", "Name", "ChannelName", "CreatedDate" };

    public GetAllStoresQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0).WithMessage("PageNumber must be at least 0.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be at least 1.")
            .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100."); // Example max page size

        RuleFor(x => x.SortBy)
            .Must(sortBy => string.IsNullOrWhiteSpace(sortBy) || _allowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage(x => $"SortBy must be one of the allowed fields: {string.Join(", ", _allowedSortFields)}. You provided '{x.SortBy}'.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortOrder)
            .Must(sortOrder => string.IsNullOrWhiteSpace(sortOrder) || new[] { "ASC", "DESC" }.Contains(sortOrder, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be 'ASC' or 'DESC'.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortOrder));

        RuleFor(x => x.FilterQuery)
            .MaximumLength(100).WithMessage("FilterQuery cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FilterQuery));
    }
}