using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Commands.CreateBox;

public record CreateBoxCommand(string? Name, string? Description, string? ImageBase64, List<CreateItemRequest>? Items) : IRequest<BoxDto>;

public record CreateItemRequest(string Name, string? Description);
