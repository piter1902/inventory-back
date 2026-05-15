using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Infrastructure.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BoxInventory.Infrastructure.Repositories;

public class BoxRepository : BaseRepository<Box>, IBoxRepository
{
    public BoxRepository(MongoDbContext context)
        : base(context.Boxes)
    {
    }

    public async Task<bool> ExistsByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await Collection
            .Find(b => b.Identifier == identifier)
            .AnyAsync(cancellationToken);
    }

    public async Task<List<Box>> SearchByNameAsync(string query, CancellationToken cancellationToken = default)
    {
        var regex = new BsonRegularExpression(query, "i");

        var filter = Builders<Box>.Filter.Or(
            Builders<Box>.Filter.Regex(b => b.Name, regex),
            Builders<Box>.Filter.Regex(b => b.Description, regex),
            Builders<Box>.Filter.ElemMatch(b => b.Items, Builders<Item>.Filter.Or(
                Builders<Item>.Filter.Regex(i => i.Name, regex),
                Builders<Item>.Filter.Regex(i => i.Description, regex))));

        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public override async Task<List<Box>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Collection
            .Find(_ => true)
            .SortBy(b => b.Identifier)
            .ToListAsync(cancellationToken);
    }
}
