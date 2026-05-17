using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.Search;

public class SearchQueryHandler : IRequestHandler<SearchQuery, SearchResultDto>
{
    private readonly IBoxRepository _repository;
    private readonly IZoneRepository _zoneRepository;

    public SearchQueryHandler(IBoxRepository repository, IZoneRepository zoneRepository)
    {
        _repository = repository;
        _zoneRepository = zoneRepository;
    }

    public async Task<SearchResultDto> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return new SearchResultDto([], []);

        var boxes = await _repository.SearchByNameAsync(request.Query, cancellationToken);

        var zones = await _zoneRepository.GetAllAsync(cancellationToken);
        var zoneMap = zones.ToDictionary(z => z.Id, z => z.Name);

        var matchingBoxes = boxes
            .Where(b => b.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                     || b.Description.Contains(request.Query, StringComparison.OrdinalIgnoreCase))
            .Select(b => new SearchBoxResultDto(
                b.Id.ToString(),
                b.Name,
                b.Description,
                b.ImageBase64,
                zoneMap.GetValueOrDefault(b.ZoneId)))
            .ToList();

        var matchingItems = boxes
            .SelectMany(b => b.Items
                .Where(i => i.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                         || i.Description.Contains(request.Query, StringComparison.OrdinalIgnoreCase))
                .Select(i => new SearchItemResultDto(
                    i.Id.ToString(),
                    i.Name,
                    i.Description,
                    b.Id.ToString(),
                    b.Name,
                    zoneMap.GetValueOrDefault(b.ZoneId))))
            .ToList();

        return new SearchResultDto(matchingBoxes, matchingItems);
    }
}
