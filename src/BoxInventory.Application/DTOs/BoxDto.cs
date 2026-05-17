namespace BoxInventory.Application.DTOs;

public record BoxDto(
    string Id,
    string Identifier,
    string Name,
    string Description,
    string QrUrl,
    string ImageBase64,
    string ZoneId,
    string? ZoneName,
    List<ItemDto> Items);
