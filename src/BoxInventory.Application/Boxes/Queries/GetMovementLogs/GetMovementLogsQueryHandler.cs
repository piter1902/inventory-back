using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Queries.GetMovementLogs;

public class GetMovementLogsQueryHandler : IRequestHandler<GetMovementLogsQuery, List<ItemMovementLogDto>>
{
    private readonly IItemMovementLogRepository _logRepository;

    public GetMovementLogsQueryHandler(IItemMovementLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<List<ItemMovementLogDto>> Handle(GetMovementLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = !string.IsNullOrWhiteSpace(request.BoxId)
            ? await _logRepository.GetByBoxIdAsync(request.BoxId, cancellationToken)
            : await _logRepository.GetAllAsync(cancellationToken);

        return logs.Select(l => new ItemMovementLogDto(
            l.Id.ToString(),
            l.ItemId,
            l.ItemName,
            l.SourceBoxId,
            l.SourceBoxName,
            l.DestinationBoxId,
            l.DestinationBoxName,
            l.MovedBy,
            l.MovedAt)).ToList();
    }
}
