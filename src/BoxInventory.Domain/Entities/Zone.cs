using BoxInventory.Domain.Common;
using MongoDB.Bson;

namespace BoxInventory.Domain.Entities;

public class Zone : IEntity
{
    public const string DefaultZoneName = "Sin especificar";

    public ObjectId Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Zone() { }

    public Zone(string name)
    {
        Id = ObjectId.GenerateNewId();
        SetName(name);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Zone name cannot be empty", nameof(name));
        Name = name;
    }
}
