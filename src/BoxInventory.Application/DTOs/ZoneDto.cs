namespace BoxInventory.Application.DTOs;

public record ZoneDto(string Id, string Name);

public record ZoneDetailDto(string Id, string Name, List<BoxDto> Boxes);
