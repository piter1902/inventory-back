using BoxInventory.Domain.Entities;

namespace BoxInventory.Domain.Interfaces;

public interface IZoneRepository : IBaseRepository<Zone>
{
    Task<Zone?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
