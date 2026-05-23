using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetMovementLogs;

public record GetMovementLogsQuery(string? BoxId) : IRequest<List<ItemMovementLogDto>>;
