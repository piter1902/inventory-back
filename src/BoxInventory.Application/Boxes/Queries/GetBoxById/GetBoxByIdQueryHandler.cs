using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetBoxById;

public class GetBoxByIdQueryHandler : IRequestHandler<GetBoxByIdQuery, BoxDto>
{
    private readonly IBoxRepository _repository;

    public GetBoxByIdQueryHandler(IBoxRepository repository)
    {
        _repository = repository;
    }

    public async Task<BoxDto> Handle(GetBoxByIdQuery request, CancellationToken cancellationToken)
    {
        var box = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Box), request.Id);

        return new BoxDto(
            box.Id.ToString(),
            box.Identifier,
            box.Name,
            box.Description,
            box.QrUrl,
            box.ImageBase64,
            box.Items.Select(i => new ItemDto(i.Id.ToString(), i.Name, i.Description)).ToList());
    }
}
