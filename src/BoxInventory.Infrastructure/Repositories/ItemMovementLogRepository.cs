using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Infrastructure.Persistence;
using MongoDB.Driver;

namespace BoxInventory.Infrastructure.Repositories;

public class ItemMovementLogRepository : BaseRepository<ItemMovementLog>, IItemMovementLogRepository
{
    public ItemMovementLogRepository(MongoDbContext context)
        : base(context.GetCollection<ItemMovementLog>("item_movement_logs"))
    {
    }

    public async Task<List<ItemMovementLog>> GetByBoxIdAsync(string boxId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ItemMovementLog>.Filter.Or(
            Builders<ItemMovementLog>.Filter.Eq(l => l.SourceBoxId, boxId),
            Builders<ItemMovementLog>.Filter.Eq(l => l.DestinationBoxId, boxId));

        return await Collection.Find(filter)
            .SortByDescending(l => l.MovedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateBoxNameAsync(string boxId, string newName, CancellationToken cancellationToken = default)
    {
        var sourceUpdate = Builders<ItemMovementLog>.Update
            .Set(l => l.SourceBoxName, newName);

        await Collection.UpdateManyAsync(
            Builders<ItemMovementLog>.Filter.Eq(l => l.SourceBoxId, boxId),
            sourceUpdate,
            cancellationToken: cancellationToken);

        var destUpdate = Builders<ItemMovementLog>.Update
            .Set(l => l.DestinationBoxName, newName);

        await Collection.UpdateManyAsync(
            Builders<ItemMovementLog>.Filter.Eq(l => l.DestinationBoxId, boxId),
            destUpdate,
            cancellationToken: cancellationToken);
    }
}
