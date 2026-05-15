namespace BoxInventory.Application.DTOs;

public record BoxDto(
    string Id,
    string Identifier,
    string Name,
    string QrUrl,
    string ImageBase64,
    List<ItemDto> Items);
