using BoxInventory.Domain.Interfaces;
using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetAllBoxes;

public class GetAllBoxesQueryHandler : IRequestHandler<GetAllBoxesQuery, List<BoxDto>>
{
    private readonly IBoxRepository _repository;

    public GetAllBoxesQueryHandler(IBoxRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BoxDto>> Handle(GetAllBoxesQuery request, CancellationToken cancellationToken)
    {
        var boxes = await _repository.GetAllAsync(cancellationToken);

        return boxes.Select(b => new BoxDto(
            b.Id.ToString(),
            b.Identifier,
            b.Name,
            b.QrUrl,
            b.ImageBase64,
            b.Items.Select(i => new ItemDto(i.Id.ToString(), i.Name, i.Description)).ToList()
        )).ToList();
    }
}
