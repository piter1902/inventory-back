using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Zones.Queries.GetAllZones;

public record GetAllZonesQuery : IRequest<List<ZoneDto>>;
