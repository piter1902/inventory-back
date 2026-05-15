using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Zones.Commands.CreateZone;

public record CreateZoneCommand(string Name) : IRequest<ZoneDto>;
