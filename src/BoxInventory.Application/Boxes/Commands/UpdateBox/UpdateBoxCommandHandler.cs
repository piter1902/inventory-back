using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Application.Common.Interfaces;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Application.DTOs;
using MediatR;
using BoxInventory.Domain.Entities;
using MongoDB.Bson;

namespace BoxInventory.Application.Boxes.Commands.UpdateBox;

public class UpdateBoxCommandHandler : IRequestHandler<UpdateBoxCommand, BoxDto>
{
    private readonly IBoxRepository _repository;
    private readonly IImageCompressionService _imageCompression;
    private readonly IZoneRepository _zoneRepository;

    public UpdateBoxCommandHandler(IBoxRepository repository, IImageCompressionService imageCompression, IZoneRepository zoneRepository)
    {
        _repository = repository;
        _imageCompression = imageCompression;
        _zoneRepository = zoneRepository;
    }

    public async Task<BoxDto> Handle(UpdateBoxCommand request, CancellationToken cancellationToken)
    {
        var box = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Box), request.Id);

        box.SetName(request.Name);
        box.SetDescription(request.Description);
        box.SetImageBase64(_imageCompression.Compress(request.ImageBase64));

        if (request.ZoneId is not null)
        {
            box.SetZone(ObjectId.Parse(request.ZoneId));
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

        return new BoxDto(
            box.Id.ToString(),
            box.Identifier,
            box.Name,
            box.Description,
            box.QrUrl,
            box.ImageBase64,
            box.ZoneId.ToString(),
            box.Items.Select(i => new ItemDto(i.Id.ToString(), i.Name, i.Description)).ToList());
    }
}
