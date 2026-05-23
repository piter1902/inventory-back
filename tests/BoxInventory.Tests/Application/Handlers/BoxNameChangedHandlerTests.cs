using BoxInventory.Application.Boxes.Notifications;
using BoxInventory.Domain.Interfaces;

namespace BoxInventory.Tests.Application.Handlers;

public class BoxNameChangedHandlerTests
{
    private readonly Mock<IItemMovementLogRepository> _logRepository;
    private readonly BoxNameChangedHandler _handler;

    public BoxNameChangedHandlerTests()
    {
        _logRepository = new Mock<IItemMovementLogRepository>();
        _handler = new BoxNameChangedHandler(_logRepository.Object);
    }

    [Fact]
    public async Task Handle_CallsUpdateBoxNameAsync()
    {
        var notification = new BoxNameChangedNotification("box123", "New Box Name");

        await _handler.Handle(notification, default);

        _logRepository.Verify(r => r.UpdateBoxNameAsync("box123", "New Box Name", It.IsAny<CancellationToken>()), Times.Once);
    }
}
