using BoxInventory.Domain.Common;
using MongoDB.Bson;

namespace BoxInventory.Domain.Entities;

public class Item : IEntity
{
    public ObjectId Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private Item() { }

    public Item(string name, string description)
    {
        Id = ObjectId.GenerateNewId();
        SetName(name);
        SetDescription(description);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Item name cannot be empty", nameof(name));

        Name = name;
    }

    public void SetDescription(string description)
    {
        Description = description ?? string.Empty;
    }
}
