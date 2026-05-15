using FluentValidation;

namespace BoxInventory.Application.Zones.Commands.CreateZone;

public class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
{
    public CreateZoneCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
