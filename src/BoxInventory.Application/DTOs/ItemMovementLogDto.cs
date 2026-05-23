namespace BoxInventory.Application.DTOs;

public record ItemMovementLogDto(
    string Id,
    string ItemId,
    string ItemName,
    string SourceBoxId,
    string SourceBoxName,
    string DestinationBoxId,
    string DestinationBoxName,
    string MovedBy,
    DateTime MovedAt);
