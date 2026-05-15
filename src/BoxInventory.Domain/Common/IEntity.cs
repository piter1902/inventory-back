using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BoxInventory.Domain.Common;

public interface IEntity
{
    [BsonId]
    ObjectId Id { get; }
}
