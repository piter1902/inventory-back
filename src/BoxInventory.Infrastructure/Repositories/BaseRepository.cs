using BoxInventory.Domain.Common;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Infrastructure.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BoxInventory.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity
{
    protected readonly IMongoCollection<T> Collection;

    public BaseRepository(IMongoCollection<T> collection)
    {
        Collection = collection;
    }

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var objectId = ObjectId.Parse(id);
        return await Collection.Find(Builders<T>.Filter.Eq("_id", objectId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Collection.ReplaceOneAsync(
            Builders<T>.Filter.Eq("_id", entity.Id),
            entity,
            cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var objectId = ObjectId.Parse(id);
        await Collection.DeleteOneAsync(
            Builders<T>.Filter.Eq("_id", objectId),
            cancellationToken);
    }
}
