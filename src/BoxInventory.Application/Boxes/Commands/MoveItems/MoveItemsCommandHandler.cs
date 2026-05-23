using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace BoxInventory.Application.Boxes.Commands.MoveItems;

public class MoveItemsCommandHandler : IRequestHandler<MoveItemsCommand, MoveItemsResult>
{
    private readonly IBoxRepository _boxRepository;
    private readonly IItemMovementLogRepository _logRepository;

    public MoveItemsCommandHandler(IBoxRepository boxRepository, IItemMovementLogRepository logRepository)
    {
        _boxRepository = boxRepository;
        _logRepository = logRepository;
    }

    public async Task<MoveItemsResult> Handle(MoveItemsCommand request, CancellationToken cancellationToken)
    {
        var sourceBox = await _boxRepository.GetByIdAsync(request.SourceBoxId, cancellationToken)
            ?? throw new NotFoundException(nameof(Box), request.SourceBoxId);

        var destBox = await _boxRepository.GetByIdAsync(request.DestinationBoxId, cancellationToken)
            ?? throw new NotFoundException(nameof(Box), request.DestinationBoxId);

        var results = new List<ItemMoveResult>();
        var logs = new List<ItemMovementLog>();

        foreach (var itemIdStr in request.ItemIds)
        {
            var itemObjectId = ObjectId.Parse(itemIdStr);
            var item = sourceBox.Items.FirstOrDefault(i => i.Id == itemObjectId);

            if (item is null)
            {
                results.Add(new ItemMoveResult(itemIdStr, string.Empty, false, $"Item with id {itemIdStr} not found in source box"));
                continue;
            }

            var itemName = item.Name;
            var itemDescription = item.Description;

            sourceBox.RemoveItem(itemObjectId);
            destBox.AddItem(new Item(itemName, itemDescription));

            var log = new ItemMovementLog(
                itemIdStr,
                itemName,
                sourceBox.Id.ToString(),
                sourceBox.Name,
                destBox.Id.ToString(),
                destBox.Name,
                request.MovedBy);

            logs.Add(log);
            results.Add(new ItemMoveResult(itemIdStr, itemName, true, null));
        }

        await _boxRepository.UpdateAsync(sourceBox, cancellationToken);
        await _boxRepository.UpdateAsync(destBox, cancellationToken);

        foreach (var log in logs)
        {
            await _logRepository.CreateAsync(log, cancellationToken);
        }

        return new MoveItemsResult(
            request.ItemIds.Count,
            results.Count(r => r.Success),
            results.Count(r => !r.Success),
            results);
    }
}
