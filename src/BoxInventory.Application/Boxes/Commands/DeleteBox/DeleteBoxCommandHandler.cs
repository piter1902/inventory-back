using BoxInventory.Application.Common.Exceptions;
using BoxInventory.Domain.Interfaces;
using MediatR;

namespace BoxInventory.Application.Boxes.Commands.DeleteBox;

public class DeleteBoxCommandHandler : IRequestHandler<DeleteBoxCommand, Unit>
{
    private readonly IBoxRepository _repository;

    public DeleteBoxCommandHandler(IBoxRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteBoxCommand request, CancellationToken cancellationToken)
    {
        var box = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Box), request.Id);

        await _repository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}
