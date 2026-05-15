using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetAllBoxes;

public record GetAllBoxesQuery : IRequest<List<BoxDto>>;
