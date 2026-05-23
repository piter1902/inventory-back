using FluentValidation;

namespace BoxInventory.Application.Boxes.Commands.MoveItems;

public class MoveItemsCommandValidator : AbstractValidator<MoveItemsCommand>
{
    public MoveItemsCommandValidator()
    {
        RuleFor(v => v.SourceBoxId)
            .NotEmpty()
            .MaximumLength(24);

        RuleFor(v => v.DestinationBoxId)
            .NotEmpty()
            .MaximumLength(24)
            .NotEqual(v => v.SourceBoxId)
            .WithMessage("Destination box must be different from source box");

        RuleFor(v => v.ItemIds)
            .NotEmpty()
            .WithMessage("At least one item must be selected");

        RuleForEach(v => v.ItemIds)
            .NotEmpty()
            .MaximumLength(24);

        RuleFor(v => v.MovedBy)
            .NotEmpty()
            .MaximumLength(200);
    }
}
