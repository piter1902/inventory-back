using BoxInventory.Domain.Common;
using MongoDB.Bson;

namespace BoxInventory.Domain.Entities;

public class Box : IEntity
{
    public ObjectId Id { get; private set; }
    public string Identifier { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string QrUrl { get; private set; } = null!;
    public string ImageBase64 { get; private set; } = null!;
    public List<Item> Items { get; private set; } = null!;

    private Box() { }

    public Box(string identifier, string? name, string? imageBase64)
    {
        Id = ObjectId.GenerateNewId();
        SetIdentifier(identifier);
        SetName(name);
        SetImageBase64(imageBase64);
        Items = [];
    }

    public void SetIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be empty", nameof(identifier));

        Identifier = identifier;
        QrUrl = $"/box/{identifier}";
    }

    public void SetName(string? name)
    {
        Name = name ?? string.Empty;
    }

    public void SetImageBase64(string? imageBase64)
    {
        ImageBase64 = imageBase64 ?? string.Empty;
    }

    public void AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);
        Items.Add(item);
    }

    public void RemoveItem(ObjectId itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException($"Item with id {itemId} not found in box");

        Items.Remove(item);
    }

    public void UpdateItem(ObjectId itemId, string name, string description)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException($"Item with id {itemId} not found in box");

        item.SetName(name);
        item.SetDescription(description);
    }
}
