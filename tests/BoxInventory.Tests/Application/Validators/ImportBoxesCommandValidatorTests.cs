using BoxInventory.Application.Boxes.Commands.ImportBoxes;

namespace BoxInventory.Tests.Application.Validators;

public class ImportBoxesCommandValidatorTests
{
    private readonly ImportBoxesCommandValidator _validator;

    public ImportBoxesCommandValidatorTests()
    {
        _validator = new ImportBoxesCommandValidator();
    }

    [Fact]
    public void Validate_WithValidBase64_ShouldBeValid()
    {
        var command = new ImportBoxesCommand(Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyString_ShouldBeInvalid()
    {
        var command = new ImportBoxesCommand("");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidBase64_ShouldBeInvalid()
    {
        var command = new ImportBoxesCommand("not-a-valid-base64!!!");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNull_ShouldBeInvalid()
    {
        var command = new ImportBoxesCommand(null!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
