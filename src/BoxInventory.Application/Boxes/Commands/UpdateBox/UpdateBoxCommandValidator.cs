using FluentValidation;

namespace BoxInventory.Application.Boxes.Commands.UpdateBox;

public class UpdateBoxCommandValidator : AbstractValidator<UpdateBoxCommand>
{
    public UpdateBoxCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Name)
            .MaximumLength(200);

        RuleFor(v => v.Description)
            .MaximumLength(2000);

        RuleFor(v => v.ImageBase64)
            .MaximumLength(10_000_000);

        RuleForEach(v => v.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.Name)
                    .NotEmpty()
                    .MaximumLength(200);

                item.RuleFor(i => i.Description)
                    .MaximumLength(1000);
            });
    }
}
