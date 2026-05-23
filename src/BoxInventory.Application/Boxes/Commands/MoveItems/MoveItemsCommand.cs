using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Commands.MoveItems;

public record MoveItemsCommand(
    string SourceBoxId,
    List<string> ItemIds,
    string DestinationBoxId,
    string MovedBy
) : IRequest<MoveItemsResult>;
