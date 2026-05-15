using BoxInventory.Application.Common.Interfaces;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Commands.CreateBox;

public class CreateBoxCommandHandler : IRequestHandler<CreateBoxCommand, BoxDto>
{
    private readonly IBoxRepository _repository;
    private readonly IImageCompressionService _imageCompression;

    public CreateBoxCommandHandler(IBoxRepository repository, IImageCompressionService imageCompression)
    {
        _repository = repository;
        _imageCompression = imageCompression;
    }

    public async Task<BoxDto> Handle(CreateBoxCommand request, CancellationToken cancellationToken)
    {
        var identifier = $"BOX-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var compressedImage = _imageCompression.Compress(request.ImageBase64);
        var box = new Box(identifier, request.Name, compressedImage);

        if (request.Items is not null)
        {
            foreach (var item in request.Items)
            {
                box.AddItem(new Item(item.Name, item.Description ?? string.Empty));
            }
        }

        await _repository.CreateAsync(box, cancellationToken);

        return MapToDto(box);
    }

    private static BoxDto MapToDto(Box box) => new(
        box.Id.ToString(),
        box.Identifier,
        box.Name,
        box.QrUrl,
        box.ImageBase64,
        box.Items.Select(i => new ItemDto(i.Id.ToString(), i.Name, i.Description)).ToList());
}
