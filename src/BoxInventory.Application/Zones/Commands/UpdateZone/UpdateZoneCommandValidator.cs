using FluentValidation;
using MongoDB.Bson;

namespace BoxInventory.Application.Zones.Commands.UpdateZone;

public class UpdateZoneCommandValidator : AbstractValidator<UpdateZoneCommand>
{
    public UpdateZoneCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleForEach(v => v.BoxIds)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Each box ID must be a valid ObjectId.");
    }
}
