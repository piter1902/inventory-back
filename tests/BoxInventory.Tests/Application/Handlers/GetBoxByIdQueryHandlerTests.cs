using BoxInventory.Application.Boxes.Queries.GetBoxById;
using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;

namespace BoxInventory.Tests.Application.Handlers;

public class GetBoxByIdQueryHandlerTests
{
    private readonly Mock<IBoxRepository> _repository;
    private readonly GetBoxByIdQueryHandler _handler;

    public GetBoxByIdQueryHandlerTests()
    {
        _repository = new Mock<IBoxRepository>();
        _handler = new GetBoxByIdQueryHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_ExistingBox_ReturnsBox()
    {
        var id = "507f1f77bcf86cd799439011";
        var box = new Box("BOX-001", "Caja", null);
        box.AddItem(new Item("Cable", "desc"));
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(box);

        var result = await _handler.Handle(new GetBoxByIdQuery(id), default);

        result.Identifier.Should().Be("BOX-001");
        result.Name.Should().Be("Caja");
        result.Items.Should().ContainSingle(i => i.Name == "Cable");
    }

    [Fact]
    public async Task Handle_NonExistentBox_Throws()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Box?)null);

        Func<Task> act = () => _handler.Handle(new GetBoxByIdQuery("507f1f77bcf86cd799439011"), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
