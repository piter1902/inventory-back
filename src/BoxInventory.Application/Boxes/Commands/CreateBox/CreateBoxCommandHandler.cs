using BoxInventory.Application.Common.Interfaces;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace BoxInventory.Application.Boxes.Commands.CreateBox;

public class CreateBoxCommandHandler : IRequestHandler<CreateBoxCommand, BoxDto>
{
    private readonly IBoxRepository _repository;
    private readonly IZoneRepository _zoneRepository;
    private readonly IImageCompressionService _imageCompression;

    public CreateBoxCommandHandler(IBoxRepository repository, IZoneRepository zoneRepository, IImageCompressionService imageCompression)
    {
        _repository = repository;
        _zoneRepository = zoneRepository;
        _imageCompression = imageCompression;
    }

    public async Task<BoxDto> Handle(CreateBoxCommand request, CancellationToken cancellationToken)
    {
        var identifier = $"BOX-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var compressedImage = _imageCompression.Compress(request.ImageBase64);

        ObjectId zoneId;
        if (request.ZoneId is not null)
        {
            zoneId = ObjectId.Parse(request.ZoneId);
        }
        else
        {
            var defaultZone = await _zoneRepository.GetByNameAsync("Sin especificar", cancellationToken)
                ?? throw new InvalidOperationException("Default zone 'Sin especificar' not found");
            zoneId = defaultZone.Id;
        }

        var box = new Box(identifier, request.Name, request.Description, compressedImage, zoneId);

        if (request.Items is not null)
        {
            foreach (var item in request.Items)
            {
                box.AddItem(new Item(item.Name, item.Description ?? string.Empty));
            }
        }

        await _repository.CreateAsync(box, cancellationToken);

        var zoneName = (await _zoneRepository.GetByIdAsync(zoneId.ToString(), cancellationToken))?.Name;
        return MapToDto(box, zoneName);
    }

    private static BoxDto MapToDto(Box box, string? zoneName) => new(
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
