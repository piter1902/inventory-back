using BoxInventory.Application.Boxes.Commands.ImportBoxes;

namespace BoxInventory.Tests.Application.Handlers;

public class BoxSheetBuilderTests
{
    [Fact]
    public void Build_WithValidData_ReturnsBox()
    {
        var builder = new BoxSheetBuilder();
        builder.StartSheet("Caja1");
        builder.ReadCell(1, "A", "Caja:");
        builder.ReadCell(1, "B", "Caja de herramientas");
        builder.ReadCell(2, "A", "Zona:");
        builder.ReadCell(2, "B", "Taller");
        builder.ReadCell(3, "A", "Items:");
        builder.ReadCell(4, "B", "Martillo");
        builder.ReadCell(4, "C", "Martillo de 500g");
        builder.ReadCell(5, "B", "Destornillador");
        builder.ReadCell(5, "C", "Destornillador plano");

        var box = builder.Build();

        box.Should().NotBeNull();
        box!.Name.Should().Be("Caja de herramientas");
        box.Items.Should().HaveCount(2);
        box.Items.Should().Contain(i => i.Name == "Martillo" && i.Description == "Martillo de 500g");
        box.Items.Should().Contain(i => i.Name == "Destornillador" && i.Description == "Destornillador plano");
    }

    [Fact]
    public void Build_WithoutItems_CreatesBoxWithEmptyItems()
    {
        var builder = new BoxSheetBuilder();
        builder.StartSheet("Vacia");
        builder.ReadCell(1, "B", "Caja vacía");

        var box = builder.Build();

        box.Should().NotBeNull();
        box!.Name.Should().Be("Caja vacía");
        box.Items.Should().BeEmpty();
    }

    [Fact]
    public void Build_WithMissingName_ReturnsNull()
    {
        var builder = new BoxSheetBuilder();
        builder.StartSheet("SinNombre");

        var box = builder.Build();

        box.Should().BeNull();
    }

    [Fact]
    public void Build_WithAnyNonEmptyName_CreatesBox()
    {
        var builder = new BoxSheetBuilder();
        builder.StartSheet("Template");
        builder.ReadCell(1, "B", "Nombre de caja");
        builder.ReadCell(4, "B", "Item1 nombre");

        var box = builder.Build();

        box.Should().NotBeNull();
        box!.Name.Should().Be("Nombre de caja");
    }

    [Fact]
    public void Build_IncludesAllItemsWithName()
    {
        var builder = new BoxSheetBuilder();
        builder.StartSheet("Template");
        builder.ReadCell(1, "B", "Mi caja");
        builder.ReadCell(4, "B", "Item1 nombre");
        builder.ReadCell(4, "C", "Item1 descripción");
        builder.ReadCell(5, "B", "Item real");
        builder.ReadCell(5, "C", "Descripción real");

        var box = builder.Build();

        box.Should().NotBeNull();
        box!.Items.Should().HaveCount(2);
        box.Items.Should().Contain(i => i.Name == "Item1 nombre" && i.Description == "Item1 descripción");
        box.Items.Should().Contain(i => i.Name == "Item real" && i.Description == "Descripción real");
    }

    [Fact]
    public void Results_TracksErrors()
    {
        var builder = new BoxSheetBuilder();

        builder.StartSheet("Hoja1");
        builder.Build();

        builder.StartSheet("Hoja2");
        builder.ReadCell(1, "B", "Caja válida");
        builder.Build();

        builder.Results.Should().HaveCount(2);
        builder.Results[0].Error.Should().NotBeNull();
        builder.Results[1].Error.Should().BeNull();
        builder.Results[1].SheetName.Should().Be("Hoja2");
        builder.Results[1].ZoneName.Should().BeNull();
    }

    [Fact]
    public void Results_TracksZoneNameFromSheet()
    {
        var builder = new BoxSheetBuilder();

        builder.StartSheet("ConZona");
        builder.ReadCell(1, "B", "Mi caja");
        builder.ReadCell(2, "B", "Almacén Norte");
        builder.Build();

        builder.Results[0].ZoneName.Should().Be("Almacén Norte");
    }

    [Fact]
    public void MultipleSheets_ProducesMultipleResults()
    {
        var builder = new BoxSheetBuilder();

        builder.StartSheet("Caja A");
        builder.ReadCell(1, "B", "Caja A");
        builder.Build();

        builder.StartSheet("Caja B");
        builder.ReadCell(1, "B", "Caja B");
        builder.Build();

        builder.Results.Should().HaveCount(2);
        builder.Results[0].SheetName.Should().Be("Caja A");
        builder.Results[1].SheetName.Should().Be("Caja B");
    }

    [Fact]
    public void ReadCell_WithNullValue_DoesNothing()
    {
        var builder = new BoxSheetBuilder();
        builder.StartSheet("Test");
        builder.ReadCell(1, "B", null);

        var box = builder.Build();

        box.Should().BeNull();
    }

    [Fact]
    public void ReadCell_WithEmptyValue_DoesNothing()
    {
        var builder = new BoxSheetBuilder();
        builder.StartSheet("Test");
        builder.ReadCell(1, "B", "");

        var box = builder.Build();

        box.Should().BeNull();
    }
}
