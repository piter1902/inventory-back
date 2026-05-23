using FluentValidation;

namespace BoxInventory.Application.Boxes.Commands.ImportBoxes;

public class ImportBoxesCommandValidator : AbstractValidator<ImportBoxesCommand>
{
    public ImportBoxesCommandValidator()
    {
        RuleFor(v => v.FileBase64)
            .NotEmpty()
            .Must(value =>
            {
                try
                {
                    Convert.FromBase64String(value);
                    return true;
                }
                catch
                {
                    return false;
                }
            })
            .WithMessage("The value is not a valid base64 string.");

        RuleFor(v => v.FileBase64)
            .MaximumLength(50_000_000)
            .WithMessage("File exceeds the maximum allowed size of 50 MB.");
    }
}
