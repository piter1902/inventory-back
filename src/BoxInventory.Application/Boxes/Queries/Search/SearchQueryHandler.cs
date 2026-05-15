using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.Search;

public class SearchQueryHandler : IRequestHandler<SearchQuery, SearchResultDto>
{
    private readonly IBoxRepository _repository;

    public SearchQueryHandler(IBoxRepository repository)
    {
        _repository = repository;
    }

    public async Task<SearchResultDto> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return new SearchResultDto([], []);

        var boxes = await _repository.SearchByNameAsync(request.Query, cancellationToken);

        var matchingBoxes = boxes
            .Where(b => b.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase))
            .Select(b => new SearchBoxResultDto(b.Id.ToString(), b.Name, b.ImageBase64))
            .ToList();

        var matchingItems = boxes
            .SelectMany(b => b.Items
                .Where(i => i.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase))
                .Select(i => new SearchItemResultDto(
                    i.Id.ToString(),
                    i.Name,
                    i.Description,
                    b.Id.ToString(),
                    b.Name)))
            .ToList();

        return new SearchResultDto(matchingBoxes, matchingItems);
    }
}
