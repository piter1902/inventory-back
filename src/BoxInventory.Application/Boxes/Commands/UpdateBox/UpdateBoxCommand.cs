using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Commands.UpdateBox;

public record UpdateBoxCommand(string Id, string? Name, string? ImageBase64, List<UpdateItemRequest>? Items) : IRequest<BoxDto>;

public record UpdateItemRequest(string Name, string? Description);
