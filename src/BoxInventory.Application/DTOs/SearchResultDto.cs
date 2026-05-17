namespace BoxInventory.Application.DTOs;

public record SearchResultDto(
    List<SearchBoxResultDto> Boxes,
    List<SearchItemResultDto> Items);

public record SearchBoxResultDto(
    string Id,
    string Name,
    string Description,
    string ImageBase64,
    string? ZoneName);

public record SearchItemResultDto(
    string Id,
    string Name,
    string Description,
    string BoxId,
    string BoxName,
    string? ZoneName);
