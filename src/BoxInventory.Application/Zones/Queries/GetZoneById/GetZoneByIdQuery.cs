using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Zones.Queries.GetZoneById;

public record GetZoneByIdQuery(string Id) : IRequest<ZoneDto>;
