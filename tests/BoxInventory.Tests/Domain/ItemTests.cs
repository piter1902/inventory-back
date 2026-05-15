using BoxInventory.Domain.Entities;
using MongoDB.Bson;

namespace BoxInventory.Tests.Domain;

public class ItemTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var item = new Item("Cable HDMI", "Cable negro de 2 metros");

        item.Name.Should().Be("Cable HDMI");
        item.Description.Should().Be("Cable negro de 2 metros");
    }

    [Fact]
    public void Create_WithNullDescription_SetsEmpty()
    {
        var item = new Item("Cable HDMI", null!);

        item.Description.Should().BeEmpty();
    }

    [Fact]
    public void Create_GeneratesId()
    {
        var item = new Item("Cable HDMI", "desc");

        item.Id.Should().NotBe(ObjectId.Empty);
    }

    [Fact]
    public void SetName_WithEmpty_Throws()
    {
        var item = new Item("Cable HDMI", "desc");

        Action act = () => item.SetName("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetName_WithWhitespace_Throws()
    {
        var item = new Item("Cable HDMI", "desc");

        Action act = () => item.SetName("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetName_WithValidValue_Updates()
    {
        var item = new Item("Cable HDMI", "desc");

        item.SetName("Cable DisplayPort");

        item.Name.Should().Be("Cable DisplayPort");
    }

    [Fact]
    public void SetDescription_WithNull_SetsEmpty()
    {
        var item = new Item("Cable HDMI", "desc");

        item.SetDescription(null!);

        item.Description.Should().BeEmpty();
    }
}
