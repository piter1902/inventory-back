using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Zones.Queries.GetZoneById;

public class GetZoneByIdQueryHandler : IRequestHandler<GetZoneByIdQuery, ZoneDto>
{
    private readonly IZoneRepository _repository;

    public GetZoneByIdQueryHandler(IZoneRepository repository)
    {
        _repository = repository;
    }

    public async Task<ZoneDto> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
    {
        var zone = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Zone), request.Id);

        return new ZoneDto(zone.Id.ToString(), zone.Name);
    }
}
