using BoxInventory.Application.Boxes.Commands.MoveItems;

namespace BoxInventory.Tests.Application.Validators;

public class MoveItemsCommandValidatorTests
{
    private readonly MoveItemsCommandValidator _validator;

    public MoveItemsCommandValidatorTests()
    {
        _validator = new MoveItemsCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var command = new MoveItemsCommand(
            "507f1f77bcf86cd799439011",
            new List<string> { "507f1f77bcf86cd799439012" },
            "507f1f77bcf86cd799439013",
            "juan.perez");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_SourceEqualsDestination_Fails()
    {
        var command = new MoveItemsCommand(
            "507f1f77bcf86cd799439011",
            new List<string> { "507f1f77bcf86cd799439012" },
            "507f1f77bcf86cd799439011",
            "juan.perez");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("different"));
    }

    [Fact]
    public void Validate_EmptyItemIds_Fails()
    {
        var command = new MoveItemsCommand(
            "507f1f77bcf86cd799439011",
            new List<string>(),
            "507f1f77bcf86cd799439013",
            "juan.perez");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyMovedBy_Fails()
    {
        var command = new MoveItemsCommand(
            "507f1f77bcf86cd799439011",
            new List<string> { "507f1f77bcf86cd799439012" },
            "507f1f77bcf86cd799439013",
            "");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptySourceId_Fails()
    {
        var command = new MoveItemsCommand(
            "",
            new List<string> { "507f1f77bcf86cd799439012" },
            "507f1f77bcf86cd799439013",
            "juan.perez");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
