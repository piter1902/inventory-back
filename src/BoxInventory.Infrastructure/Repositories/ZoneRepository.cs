using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Infrastructure.Persistence;
using MongoDB.Driver;

namespace BoxInventory.Infrastructure.Repositories;

public class ZoneRepository : BaseRepository<Zone>, IZoneRepository
{
    public ZoneRepository(MongoDbContext context)
        : base(context.Zones)
    {
    }

    public async Task<Zone?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Collection
            .Find(z => z.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
