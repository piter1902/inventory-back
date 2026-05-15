using MediatR;

namespace BoxInventory.Application.Boxes.Commands.DeleteBox;

public record DeleteBoxCommand(string Id) : IRequest<Unit>;
