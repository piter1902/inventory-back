using BoxInventory.Application.Boxes.Commands.CreateBox;
using BoxInventory.Application.Boxes.Commands.DeleteBox;
using BoxInventory.Application.Boxes.Commands.UpdateBox;
using BoxInventory.Application.Boxes.Queries.GetBoxById;

namespace BoxInventory.Tests.Application.Validators;

public class ValidatorsTests
{
    private const string ValidId = "507f1f77bcf86cd799439011";

    [Fact]
    public void CreateBoxCommand_Valid_Passes()
    {
        var validator = new CreateBoxCommandValidator();
        var command = new CreateBoxCommand("Caja", null, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateBoxCommand_WithValidItems_Passes()
    {
        var validator = new CreateBoxCommandValidator();
        var items = new List<CreateItemRequest> { new("Cable", "desc") };
        var command = new CreateBoxCommand("Caja", null, null, items);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateBoxCommand_WithEmptyItemName_Fails()
    {
        var validator = new CreateBoxCommandValidator();
        var items = new List<CreateItemRequest> { new("", "desc") };
        var command = new CreateBoxCommand("Caja", null, null, items);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateBoxCommand_Valid_Passes()
    {
        var validator = new UpdateBoxCommandValidator();
        var command = new UpdateBoxCommand(ValidId, null, null, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateBoxCommand_EmptyId_Fails()
    {
        var validator = new UpdateBoxCommandValidator();
        var command = new UpdateBoxCommand("", null, null, null, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateBoxCommand_WithValidItems_Passes()
    {
        var validator = new UpdateBoxCommandValidator();
        var items = new List<UpdateItemRequest> { new("Cable", "desc") };
        var command = new UpdateBoxCommand(ValidId, null, null, null, items);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateBoxCommand_WithEmptyItemName_Fails()
    {
        var validator = new UpdateBoxCommandValidator();
        var items = new List<UpdateItemRequest> { new("", "desc") };
        var command = new UpdateBoxCommand(ValidId, null, null, null, items);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DeleteBoxCommand_Valid_Passes()
    {
        var validator = new DeleteBoxCommandValidator();
        var command = new DeleteBoxCommand(ValidId);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteBoxCommand_EmptyId_Fails()
    {
        var validator = new DeleteBoxCommandValidator();
        var command = new DeleteBoxCommand("");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetBoxByIdQuery_Valid_Passes()
    {
        var validator = new GetBoxByIdQueryValidator();
        var query = new GetBoxByIdQuery(ValidId);

        var result = validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetBoxByIdQuery_EmptyId_Fails()
    {
        var validator = new GetBoxByIdQueryValidator();
        var query = new GetBoxByIdQuery("");

        var result = validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }
}
