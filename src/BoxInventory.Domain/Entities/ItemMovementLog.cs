using BoxInventory.Domain.Common;
using MongoDB.Bson;

namespace BoxInventory.Domain.Entities;

public class ItemMovementLog : IEntity
{
    public ObjectId Id { get; private set; }
    public string ItemId { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public string SourceBoxId { get; private set; } = null!;
    public string SourceBoxName { get; private set; } = null!;
    public string DestinationBoxId { get; private set; } = null!;
    public string DestinationBoxName { get; private set; } = null!;
    public string MovedBy { get; private set; } = null!;
    public DateTime MovedAt { get; private set; }

    private ItemMovementLog() { }

    public ItemMovementLog(
        string itemId,
        string itemName,
        string sourceBoxId,
        string sourceBoxName,
        string destinationBoxId,
        string destinationBoxName,
        string movedBy)
    {
        Id = ObjectId.GenerateNewId();
        ItemId = itemId;
        ItemName = itemName;
        SourceBoxId = sourceBoxId;
        SourceBoxName = sourceBoxName;
        DestinationBoxId = destinationBoxId;
        DestinationBoxName = destinationBoxName;
        MovedBy = movedBy;
        MovedAt = DateTime.UtcNow;
    }
}
