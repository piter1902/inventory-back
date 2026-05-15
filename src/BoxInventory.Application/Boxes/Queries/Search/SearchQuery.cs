using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.Search;

public record SearchQuery(string Query) : IRequest<SearchResultDto>;
