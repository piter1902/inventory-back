using FluentValidation;

namespace BoxInventory.Application.Zones.Queries.GetZoneById;

public class GetZoneByIdQueryValidator : AbstractValidator<GetZoneByIdQuery>
{
    public GetZoneByIdQueryValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();
    }
}
