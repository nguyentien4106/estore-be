using FluentValidation;

namespace EStore.Services.Estore.Application.Commands.Stores.AddStore;

public class AddStoreCommandValidator : AbstractValidator<AddStoreCommand>
{
    public AddStoreCommandValidator()
    {
        RuleFor(x => x.ChannelName)
            .NotEmpty().WithMessage("ChannelName is required.")
            .MaximumLength(100).WithMessage("ChannelName must not exceed 100 characters.");
    }
} 