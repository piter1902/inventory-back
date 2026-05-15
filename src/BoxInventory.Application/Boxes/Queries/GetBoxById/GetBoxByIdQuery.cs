using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetBoxById;

public record GetBoxByIdQuery(string Id) : IRequest<BoxDto>;
