using BoxInventory.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
    public IMongoCollection<Zone> Zones => _database.GetCollection<Zone>("zones");
    public IMongoCollection<ItemMovementLog> Logs => _database.GetCollection<ItemMovementLog>("item_movement_logs");

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);

    public virtual Task PingAsync(CancellationToken cancellationToken = default) =>
        _database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);

    public async Task EnsureIndexesAsync()
    {
        var boxIndexKeys = Builders<Box>.IndexKeys.Ascending(b => b.Identifier);
        var boxIndexModel = new CreateIndexModel<Box>(boxIndexKeys, new CreateIndexOptions { Unique = true });
            await Boxes.Indexes.CreateOneAsync(boxIndexModel);

        var zoneIndexKeys = Builders<Zone>.IndexKeys.Ascending(z => z.Name);
        var zoneIndexModel = new CreateIndexModel<Zone>(zoneIndexKeys, new CreateIndexOptions { Unique = true });
        await Zones.Indexes.CreateOneAsync(zoneIndexModel);

        var defaultZone = await Zones.Find(z => z.Name == Zone.DefaultZoneName).FirstOrDefaultAsync();
        if (defaultZone is null)
        {
            await Zones.InsertOneAsync(new Zone(Zone.DefaultZoneName));
        }
    }
}
