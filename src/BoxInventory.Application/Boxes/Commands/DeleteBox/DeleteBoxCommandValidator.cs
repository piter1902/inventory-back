using FluentValidation;

namespace BoxInventory.Application.Boxes.Commands.DeleteBox;

public class DeleteBoxCommandValidator : AbstractValidator<DeleteBoxCommand>
{
    public DeleteBoxCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();
    }
}
