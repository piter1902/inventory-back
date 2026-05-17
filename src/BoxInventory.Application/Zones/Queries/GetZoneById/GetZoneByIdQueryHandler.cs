using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Zones.Queries.GetZoneById;

public class GetZoneByIdQueryHandler : IRequestHandler<GetZoneByIdQuery, ZoneDetailDto>
{
    private readonly IZoneRepository _zoneRepository;
    private readonly IBoxRepository _boxRepository;

    public GetZoneByIdQueryHandler(IZoneRepository zoneRepository, IBoxRepository boxRepository)
    {
        _zoneRepository = zoneRepository;
        _boxRepository = boxRepository;
    }

    public async Task<ZoneDetailDto> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
    {
        var zone = await _zoneRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Zone), request.Id);

        var boxes = await _boxRepository.GetByZoneIdAsync(zone.Id, cancellationToken);

        return new ZoneDetailDto(
            zone.Id.ToString(),
            zone.Name,
            boxes.Select(b => new BoxDto(
                b.Id.ToString(),
                b.Identifier,
                b.Name,
                b.Description,
                b.QrUrl,
                b.ImageBase64,
                b.ZoneId.ToString(),
                zone.Name,
                b.Items.Select(i => new ItemDto(i.Id.ToString(), i.Name, i.Description)).ToList()
            )).ToList());
    }
}
