using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Zones.Commands.CreateZone;

public class CreateZoneCommandHandler : IRequestHandler<CreateZoneCommand, ZoneDto>
{
    private readonly IZoneRepository _repository;

    public CreateZoneCommandHandler(IZoneRepository repository)
    {
        _repository = repository;
    }

    public async Task<ZoneDto> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
    {
        var zone = new Zone(request.Name);
        await _repository.CreateAsync(zone, cancellationToken);

        return new ZoneDto(zone.Id.ToString(), zone.Name);
    }
}
