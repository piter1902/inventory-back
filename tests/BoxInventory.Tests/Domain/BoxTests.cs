using BoxInventory.Domain.Entities;
using MongoDB.Bson;

namespace BoxInventory.Tests.Domain;

public class BoxTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var box = new Box("BOX-001", "Caja trastero", null);

        box.Identifier.Should().Be("BOX-001");
        box.Name.Should().Be("Caja trastero");
        box.QrUrl.Should().Be("/box/BOX-001");
        box.Items.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNullName_SetsEmptyString()
    {
        var box = new Box("BOX-001", null, null);

        box.Name.Should().BeEmpty();
        box.QrUrl.Should().Be("/box/BOX-001");
    }

    [Fact]
    public void SetIdentifier_WithEmpty_Throws()
    {
        var box = new Box("BOX-001", null, null);

        Action act = () => box.SetIdentifier("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetIdentifier_WithWhitespace_Throws()
    {
        var box = new Box("BOX-001", null, null);

        Action act = () => box.SetIdentifier("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddItem_ValidItem_AddsToList()
    {
        var box = new Box("BOX-001", null, null);
        var item = new Item("Cable HDMI", "Cable negro");

        box.AddItem(item);

        box.Items.Should().ContainSingle();
    }

    [Fact]
    public void AddItem_NullItem_Throws()
    {
        var box = new Box("BOX-001", null, null);

        Action act = () => box.AddItem(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveItem_ExistingItem_RemovesFromList()
    {
        var box = new Box("BOX-001", null, null);
        var item = new Item("Cable HDMI", "Cable negro");
        box.AddItem(item);

        box.RemoveItem(item.Id);

        box.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_NonExistent_Throws()
    {
        var box = new Box("BOX-001", null, null);

        Action act = () => box.RemoveItem(ObjectId.GenerateNewId());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UpdateItem_ExistingItem_UpdatesProperties()
    {
        var box = new Box("BOX-001", null, null);
        var item = new Item("Cable HDMI", "Cable negro");
        box.AddItem(item);

        box.UpdateItem(item.Id, "Cable DisplayPort", "Cable blanco");

        var updated = box.Items.Single();
        updated.Name.Should().Be("Cable DisplayPort");
        updated.Description.Should().Be("Cable blanco");
    }

    [Fact]
    public void UpdateItem_NonExistent_Throws()
    {
        var box = new Box("BOX-001", null, null);

        Action act = () => box.UpdateItem(ObjectId.GenerateNewId(), "name", "desc");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Create_GeneratesUniqueId()
    {
        var box1 = new Box("BOX-001", null, null);
        var box2 = new Box("BOX-002", null, null);

        box1.Id.Should().NotBe(box2.Id);
    }

    [Fact]
    public void MultipleItems_AllTrackedCorrectly()
    {
        var box = new Box("BOX-001", null, null);
        var item1 = new Item("Item 1", "Desc 1");
        var item2 = new Item("Item 2", "Desc 2");
        var item3 = new Item("Item 3", "Desc 3");

        box.AddItem(item1);
        box.AddItem(item2);
        box.AddItem(item3);
        box.RemoveItem(item2.Id);

        box.Items.Should().HaveCount(2);
        box.Items.Should().Contain(i => i.Name == "Item 1");
        box.Items.Should().Contain(i => i.Name == "Item 3");
    }
}
