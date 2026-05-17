using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Zones.Commands.UpdateZone;

public record UpdateZoneCommand(string Id, string Name, List<string>? BoxIds) : IRequest<ZoneDetailDto>;
