using BoxInventory.Application.Boxes.Commands.DeleteBox;
using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;

namespace BoxInventory.Tests.Application.Handlers;

public class DeleteBoxCommandHandlerTests
{
    private readonly Mock<IBoxRepository> _repository;
    private readonly DeleteBoxCommandHandler _handler;

    public DeleteBoxCommandHandlerTests()
    {
        _repository = new Mock<IBoxRepository>();
        _handler = new DeleteBoxCommandHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_ExistingBox_Deletes()
    {
        var id = "507f1f77bcf86cd799439011";
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Box("BOX-001", null, null));

        var result = await _handler.Handle(new DeleteBoxCommand(id), default);

        _repository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentBox_Throws()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Box?)null);

        Func<Task> act = () => _handler.Handle(new DeleteBoxCommand("507f1f77bcf86cd799439011"), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
