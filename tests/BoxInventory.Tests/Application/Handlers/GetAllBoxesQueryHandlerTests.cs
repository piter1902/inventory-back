using BoxInventory.Application.Boxes.Queries.GetAllBoxes;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;

namespace BoxInventory.Tests.Application.Handlers;

public class GetAllBoxesQueryHandlerTests
{
    private readonly Mock<IBoxRepository> _repository;
    private readonly GetAllBoxesQueryHandler _handler;

    public GetAllBoxesQueryHandlerTests()
    {
        _repository = new Mock<IBoxRepository>();
        _handler = new GetAllBoxesQueryHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllBoxes()
    {
        var boxes = new List<Box>
        {
            new("BOX-001", "Caja 1", null),
            new("BOX-002", "Caja 2", null),
        };
        boxes[0].AddItem(new Item("Item 1", "desc"));
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(boxes);

        var result = await _handler.Handle(new GetAllBoxesQuery(), default);

        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Identifier == "BOX-001");
        result.Should().Contain(b => b.Identifier == "BOX-002");
    }

    [Fact]
    public async Task Handle_NoBoxes_ReturnsEmpty()
    {
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(new GetAllBoxesQuery(), default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_IncludesItemsInBoxes()
    {
        var box = new Box("BOX-001", null, null);
        box.AddItem(new Item("Cable", "desc"));
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([box]);

        var result = await _handler.Handle(new GetAllBoxesQuery(), default);

        result.Single().Items.Should().ContainSingle(i => i.Name == "Cable");
    }
}
