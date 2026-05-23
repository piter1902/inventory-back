using BoxInventory.Application.Boxes.Commands.MoveItems;
using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MongoDB.Bson;

namespace BoxInventory.Tests.Application.Handlers;

public class MoveItemsCommandHandlerTests
{
    private readonly Mock<IBoxRepository> _boxRepository;
    private readonly Mock<IItemMovementLogRepository> _logRepository;
    private readonly MoveItemsCommandHandler _handler;

    public MoveItemsCommandHandlerTests()
    {
        _boxRepository = new Mock<IBoxRepository>();
        _logRepository = new Mock<IItemMovementLogRepository>();
        _handler = new MoveItemsCommandHandler(_boxRepository.Object, _logRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidMove_MovesItemAndCreatesLog()
    {
        var sourceBox = CreateBox("BOX-AAAA", "Caja Origen");
        var item = new Item("Martillo", "Martillo de 500g");
        sourceBox.AddItem(item);

        var destBox = CreateBox("BOX-BBBB", "Caja Destino");

        _boxRepository.Setup(r => r.GetByIdAsync(sourceBox.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBox);
        _boxRepository.Setup(r => r.GetByIdAsync(destBox.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(destBox);

        var command = new MoveItemsCommand(
            sourceBox.Id.ToString(),
            new List<string> { item.Id.ToString() },
            destBox.Id.ToString(),
            "juan.perez");

        var result = await _handler.Handle(command, default);

        result.TotalItems.Should().Be(1);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(0);

        sourceBox.Items.Should().BeEmpty();
        destBox.Items.Should().HaveCount(1);
        destBox.Items[0].Name.Should().Be("Martillo");

        _boxRepository.Verify(r => r.UpdateAsync(sourceBox, It.IsAny<CancellationToken>()), Times.Once);
        _boxRepository.Verify(r => r.UpdateAsync(destBox, It.IsAny<CancellationToken>()), Times.Once);
        _logRepository.Verify(r => r.CreateAsync(It.Is<ItemMovementLog>(l =>
            l.ItemName == "Martillo" &&
            l.MovedBy == "juan.perez" &&
            l.SourceBoxId == sourceBox.Id.ToString() &&
            l.DestinationBoxId == destBox.Id.ToString()), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentSource_Throws()
    {
        _boxRepository.Setup(r => r.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Box?)null);

        var command = new MoveItemsCommand("nonexistent", new List<string> { "abc" }, "dest", "user");

        await _handler.Invoking(h => h.Handle(command, default))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NonExistentDest_Throws()
    {
        var sourceBox = CreateBox("BOX-AAAA", "Source");
        _boxRepository.Setup(r => r.GetByIdAsync(sourceBox.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBox);
        _boxRepository.Setup(r => r.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Box?)null);

        var command = new MoveItemsCommand(sourceBox.Id.ToString(), new List<string> { "abc" }, "nonexistent", "user");

        await _handler.Invoking(h => h.Handle(command, default))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ItemNotFoundInSource_ReportsFailure()
    {
        var sourceBox = CreateBox("BOX-AAAA", "Origen");
        var destBox = CreateBox("BOX-BBBB", "Destino");

        _boxRepository.Setup(r => r.GetByIdAsync(sourceBox.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBox);
        _boxRepository.Setup(r => r.GetByIdAsync(destBox.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(destBox);

        var command = new MoveItemsCommand(
            sourceBox.Id.ToString(),
            new List<string> { ObjectId.GenerateNewId().ToString() },
            destBox.Id.ToString(),
            "user");

        var result = await _handler.Handle(command, default);

        result.TotalItems.Should().Be(1);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(1);
        result.Results[0].Error.Should().Contain("not found");

        _logRepository.Verify(r => r.CreateAsync(It.IsAny<ItemMovementLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleItems_MovesAllAndCreatesLogs()
    {
        var sourceBox = CreateBox("BOX-AAAA", "Origen");
        var item1 = new Item("Item1", "Desc1");
        var item2 = new Item("Item2", "Desc2");
        sourceBox.AddItem(item1);
        sourceBox.AddItem(item2);

        var destBox = CreateBox("BOX-BBBB", "Destino");

        _boxRepository.Setup(r => r.GetByIdAsync(sourceBox.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBox);
        _boxRepository.Setup(r => r.GetByIdAsync(destBox.Id.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(destBox);

        var command = new MoveItemsCommand(
            sourceBox.Id.ToString(),
            new List<string> { item1.Id.ToString(), item2.Id.ToString() },
            destBox.Id.ToString(),
            "user");

        var result = await _handler.Handle(command, default);

        result.SuccessCount.Should().Be(2);
        sourceBox.Items.Should().BeEmpty();
        destBox.Items.Should().HaveCount(2);

        _logRepository.Verify(r => r.CreateAsync(It.IsAny<ItemMovementLog>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    private static Box CreateBox(string identifier, string name)
    {
        return new Box(identifier, name, null, null, ObjectId.Empty);
    }
}
