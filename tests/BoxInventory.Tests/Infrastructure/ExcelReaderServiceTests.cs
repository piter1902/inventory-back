using BoxInventory.Application.Boxes.Commands.ImportBoxes;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Infrastructure.Services;

namespace BoxInventory.Tests.Infrastructure;

public class ExcelReaderServiceTests
{
    private readonly IExcelReaderService _service;

    public ExcelReaderServiceTests()
    {
        _service = new ExcelReaderService();
    }

    [Fact]
    public void Read_WithTemplateFile_CreatesBoxesWithRawCellValues()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "caja_import_template.xlsx");
        var bytes = File.ReadAllBytes(filePath);

        var builder = new BoxSheetBuilder();
        var boxes = _service.Read(bytes, builder);

        boxes.Should().HaveCount(2);
        boxes[0].Name.Should().Be("Nombre de caja");
        boxes[1].Name.Should().Be("Nombre de caja");
        builder.Results.Should().HaveCount(2);
        builder.Results[0].SheetName.Should().Be("Caja1");
        builder.Results[0].Error.Should().BeNull();
        builder.Results[1].SheetName.Should().Be("Caja 2");
        builder.Results[1].Error.Should().BeNull();
    }

    [Fact]
    public void Read_WithEditedTemplate_CreatesCorrectBoxes()
    {
        var bytes = CreateExcelFromTemplate(wb =>
        {
            var ws = wb.Worksheet(1);
            ws.Cell(1, 2).Value = "Caja Real";
            ws.Cell(2, 2).Value = "Taller";
            ws.Cell(4, 2).Value = "Martillo";
            ws.Cell(4, 3).Value = "Martillo de 500g";
            ws.Cell(5, 2).Value = "Destornillador";
            ws.Cell(5, 3).Value = "Destornillador plano";
        });

        var builder = new BoxSheetBuilder();
        var boxes = _service.Read(bytes, builder);

        boxes.Should().HaveCount(2);
        var box = boxes[0];
        box.Name.Should().Be("Caja Real");
        box.Items.Should().HaveCount(2);
        box.Items.Should().Contain(i => i.Name == "Martillo" && i.Description == "Martillo de 500g");
        box.Items.Should().Contain(i => i.Name == "Destornillador" && i.Description == "Destornillador plano");
    }

    [Fact]
    public void Read_WithBothSheetsEdited_CreatesTwoBoxes()
    {
        var bytes = CreateExcelFromTemplate(wb =>
        {
            var ws1 = wb.Worksheet(1);
            ws1.Cell(1, 2).Value = "Caja Uno";
            ws1.Cell(4, 2).Value = "Item A";

            var ws2 = wb.Worksheet(2);
            ws2.Cell(1, 2).Value = "Caja Dos";
            ws2.Cell(4, 2).Value = "Item B";
        });

        var builder = new BoxSheetBuilder();
        var boxes = _service.Read(bytes, builder);

        boxes.Should().HaveCount(2);
        boxes[0].Name.Should().Be("Caja Uno");
        boxes[1].Name.Should().Be("Caja Dos");
    }

    [Fact]
    public void Read_WithEmptyBytes_Throws()
    {
        var bytes = Array.Empty<byte>();

        var action = () => _service.Read(bytes, new BoxSheetBuilder());

        action.Should().Throw<Exception>();
    }

    private static byte[] CreateExcelFromTemplate(Action<ClosedXML.Excel.XLWorkbook> edit)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "caja_import_template.xlsx");
        var bytes = File.ReadAllBytes(filePath);

        using var stream = new MemoryStream(bytes);
        using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
        edit(workbook);

        using var outStream = new MemoryStream();
        workbook.SaveAs(outStream);
        return outStream.ToArray();
    }
}
