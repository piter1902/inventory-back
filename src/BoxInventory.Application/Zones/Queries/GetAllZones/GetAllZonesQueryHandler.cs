using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Zones.Queries.GetAllZones;

public class GetAllZonesQueryHandler : IRequestHandler<GetAllZonesQuery, List<ZoneDto>>
{
    private readonly IZoneRepository _repository;

    public GetAllZonesQueryHandler(IZoneRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ZoneDto>> Handle(GetAllZonesQuery request, CancellationToken cancellationToken)
    {
        var zones = await _repository.GetAllAsync(cancellationToken);

        return zones.Select(z => new ZoneDto(z.Id.ToString(), z.Name)).ToList();
    }
}
