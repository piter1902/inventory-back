using BoxInventory.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace BoxInventory.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbOptions> options)
    {
        var pack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true),
        };
        ConventionRegistry.Register("BoxInventory", pack, t => true);

        var mongoOptions = options.Value;
        var client = new MongoClient(mongoOptions.ConnectionString);
        _database = client.GetDatabase(mongoOptions.DatabaseName);
    }

    public IMongoCollection<Box> Boxes => _database.GetCollection<Box>("boxes");

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);

    public async Task EnsureIndexesAsync()
    {
        var indexKeys = Builders<Box>.IndexKeys.Ascending(b => b.Identifier);
        var indexModel = new CreateIndexModel<Box>(indexKeys, new CreateIndexOptions { Unique = true });
        await Boxes.Indexes.CreateOneAsync(indexModel);
    }
}
