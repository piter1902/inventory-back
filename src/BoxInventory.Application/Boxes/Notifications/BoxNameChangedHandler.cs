using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Notifications;

public class BoxNameChangedHandler : INotificationHandler<BoxNameChangedNotification>
{
    private readonly IItemMovementLogRepository _logRepository;

    public BoxNameChangedHandler(IItemMovementLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task Handle(BoxNameChangedNotification notification, CancellationToken cancellationToken)
    {
        await _logRepository.UpdateBoxNameAsync(notification.BoxId, notification.NewName, cancellationToken);
    }
}
