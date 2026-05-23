using MediatR;

namespace BoxInventory.Application.Boxes.Notifications;

public record BoxNameChangedNotification(string BoxId, string NewName) : INotification;
