using BoxInventory.Application.Boxes.Notifications;
using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Application.Common.Interfaces;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace BoxInventory.Application.Boxes.Commands.UpdateBox;

public class UpdateBoxCommandHandler : IRequestHandler<UpdateBoxCommand, BoxDto>
{
    private readonly IBoxRepository _repository;
    private readonly IImageCompressionService _imageCompression;
    private readonly IZoneRepository _zoneRepository;
    private readonly IMediator _mediator;

    public UpdateBoxCommandHandler(
        IBoxRepository repository,
        IImageCompressionService imageCompression,
        IZoneRepository zoneRepository,
        IMediator mediator)
    {
        _repository = repository;
        _imageCompression = imageCompression;
        _zoneRepository = zoneRepository;
        _mediator = mediator;
    }

    public async Task<BoxDto> Handle(UpdateBoxCommand request, CancellationToken cancellationToken)
    {
        var box = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Box), request.Id);

        var oldName = box.Name;
        box.SetName(request.Name);
        var nameChanged = box.Name != oldName;

        box.SetDescription(request.Description);
        box.SetImageBase64(_imageCompression.Compress(request.ImageBase64));

        ObjectId? newZoneId = null;
        if (request.ZoneId is not null)
        {
            newZoneId = ObjectId.Parse(request.ZoneId);
            box.SetZone(newZoneId.Value);
        }

        if (request.Items is not null)
        {
            box.Items.Clear();
            foreach (var item in request.Items)
            {
                box.AddItem(new Item(item.Name, item.Description ?? string.Empty));
            }
        }

        await _repository.UpdateAsync(box, cancellationToken);

        if (nameChanged)
        {
            await _mediator.Publish(new BoxNameChangedNotification(box.Id.ToString(), box.Name), cancellationToken);
        }

        var effectiveZoneId = newZoneId ?? box.ZoneId;
        var zoneName = effectiveZoneId != ObjectId.Empty
            ? (await _zoneRepository.GetByIdAsync(effectiveZoneId.ToString(), cancellationToken))?.Name
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
