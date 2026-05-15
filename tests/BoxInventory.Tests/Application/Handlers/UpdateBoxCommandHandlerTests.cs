using BoxInventory.Application.Boxes.Commands.UpdateBox;
using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Application.Common.Interfaces;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;

namespace BoxInventory.Tests.Application.Handlers;

public class UpdateBoxCommandHandlerTests
{
    private readonly Mock<IBoxRepository> _repository;
    private readonly Mock<IImageCompressionService> _imageCompression;
    private readonly UpdateBoxCommandHandler _handler;

    public UpdateBoxCommandHandlerTests()
    {
        _repository = new Mock<IBoxRepository>();
        _imageCompression = new Mock<IImageCompressionService>();
        _imageCompression.Setup(c => c.Compress(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((string? s, int _, int __, int ___) => s);
        _handler = new UpdateBoxCommandHandler(_repository.Object, _imageCompression.Object);
    }

    [Fact]
    public async Task Handle_ExistingBox_UpdatesAndReturns()
    {
        var id = "507f1f77bcf86cd799439011";
        var box = new Box("BOX-001", "Old Name", null);
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(box);

        var result = await _handler.Handle(new UpdateBoxCommand(id, "New Name", "/new", null), default);

        result.Name.Should().Be("New Name");
        result.Identifier.Should().Be("BOX-001");
        result.QrUrl.Should().Be("/box/BOX-001");
        _repository.Verify(r => r.UpdateAsync(box, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentBox_Throws()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Box?)null);

        Func<Task> act = () => _handler.Handle(
            new UpdateBoxCommand("507f1f77bcf86cd799439011", null, null, null), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NullItems_KeepsExisting()
    {
        var id = "507f1f77bcf86cd799439011";
        var box = new Box("BOX-001", "Name", null);
        box.AddItem(new Item("Cable", "desc"));
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(box);

        var result = await _handler.Handle(new UpdateBoxCommand(id, "Updated", null, null), default);

        result.Items.Should().ContainSingle(i => i.Name == "Cable");
    }

    [Fact]
    public async Task Handle_EmptyItems_ReplacesAll()
    {
        var id = "507f1f77bcf86cd799439011";
        var box = new Box("BOX-001", "Name", null);
        box.AddItem(new Item("Cable", "desc"));
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(box);

        var result = await _handler.Handle(new UpdateBoxCommand(id, "Updated", null, []), default);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithItems_ReplacesExisting()
    {
        var id = "507f1f77bcf86cd799439011";
        var box = new Box("BOX-001", "Name", null);
        box.AddItem(new Item("Old Item", "old"));
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(box);

        var items = new List<UpdateItemRequest> { new("New Item", "new desc") };
        var result = await _handler.Handle(new UpdateBoxCommand(id, "Name", null, items), default);

        result.Items.Should().ContainSingle(i => i.Name == "New Item");
    }
}
