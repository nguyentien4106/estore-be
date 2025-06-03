using FluentValidation;

namespace EStore.Application.Commands.Stores.AddStore;

public class AddStoreRequestValidator : AbstractValidator<AddStoreRequest>
{
    public AddStoreRequestValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage("Store name cannot exceed 100 characters.");

        RuleFor(x => x.ChannelName)
            .NotEmpty().WithMessage("Channel name is required.")
            .MaximumLength(50).WithMessage("Channel name cannot exceed 50 characters.");
        // Add other validation rules if AddStoreRequest gets more properties
    }
} 