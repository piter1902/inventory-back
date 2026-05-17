using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetBoxById;

public class GetBoxByIdQueryHandler : IRequestHandler<GetBoxByIdQuery, BoxDto>
{
    private readonly IBoxRepository _repository;
    private readonly IZoneRepository _zoneRepository;

    public GetBoxByIdQueryHandler(IBoxRepository repository, IZoneRepository zoneRepository)
    {
        _repository = repository;
        _zoneRepository = zoneRepository;
    }

    public async Task<BoxDto> Handle(GetBoxByIdQuery request, CancellationToken cancellationToken)
    {
        var box = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Box), request.Id);

        var zoneName = box.ZoneId != MongoDB.Bson.ObjectId.Empty
            ? (await _zoneRepository.GetByIdAsync(box.ZoneId.ToString(), cancellationToken))?.Name
            : null;

        return new BoxDto(
            box.Id.ToString(),
            box.Identifier,
            box.Name,
            box.Description,
            box.QrUrl,
            box.ImageBase64,
            box.ZoneId.ToString(),
            zoneName,
            box.Items.Select(i => new ItemDto(i.Id.ToString(), i.Name, i.Description)).ToList());
    }
}
