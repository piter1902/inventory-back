using BoxInventory.Domain.Entities;

namespace BoxInventory.Domain.Interfaces;

public interface IItemMovementLogRepository : IBaseRepository<ItemMovementLog>
{
    Task<List<ItemMovementLog>> GetByBoxIdAsync(string boxId, CancellationToken cancellationToken = default);
    Task UpdateBoxNameAsync(string boxId, string newName, CancellationToken cancellationToken = default);
}
