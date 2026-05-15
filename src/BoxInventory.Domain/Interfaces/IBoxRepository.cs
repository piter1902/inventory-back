using BoxInventory.Domain.Entities;

namespace BoxInventory.Domain.Interfaces;

public interface IBoxRepository : IBaseRepository<Box>
{
    Task<bool> ExistsByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<List<Box>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
}
