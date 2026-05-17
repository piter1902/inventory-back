using BoxInventory.Domain.Entities;
using MongoDB.Bson;

namespace BoxInventory.Domain.Interfaces;

public interface IBoxRepository : IBaseRepository<Box>
{
    Task<bool> ExistsByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<List<Box>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
    Task<List<Box>> GetByZoneIdAsync(ObjectId zoneId, CancellationToken cancellationToken = default);
    Task AssignToZoneAsync(List<ObjectId> boxIds, ObjectId zoneId, CancellationToken cancellationToken = default);
}
