using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetAllBoxes;

public class GetAllBoxesQueryHandler : IRequestHandler<GetAllBoxesQuery, List<BoxDto>>
{
    private readonly IBoxRepository _repository;
    private readonly IZoneRepository _zoneRepository;

    public GetAllBoxesQueryHandler(IBoxRepository repository, IZoneRepository zoneRepository)
    {
        _repository = repository;
        _zoneRepository = zoneRepository;
    }

    public async Task<List<BoxDto>> Handle(GetAllBoxesQuery request, CancellationToken cancellationToken)
    {
        var boxes = await _repository.GetAllAsync(cancellationToken);

        var zones = await _zoneRepository.GetAllAsync(cancellationToken);
        var zoneMap = zones.ToDictionary(z => z.Id, z => z.Name);

        return boxes.Select(b => new BoxDto(
            b.Id.ToString(),
            b.Identifier,
            b.Name,
            b.Description,
            b.QrUrl,
            b.ImageBase64,
            b.ZoneId.ToString(),
            zoneMap.GetValueOrDefault(b.ZoneId),
            b.Items.Select(i => new ItemDto(i.Id.ToString(), i.Name, i.Description)).ToList()
        )).ToList();
    }
}
