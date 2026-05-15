using FluentValidation;

namespace BoxInventory.Application.Boxes.Queries.GetBoxById;

public class GetBoxByIdQueryValidator : AbstractValidator<GetBoxByIdQuery>
{
    public GetBoxByIdQueryValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();
    }
}
