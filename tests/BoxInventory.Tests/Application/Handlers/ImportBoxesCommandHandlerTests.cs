using BoxInventory.Application.Boxes.Commands.ImportBoxes;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MongoDB.Bson;

namespace BoxInventory.Tests.Application.Handlers;

public class ImportBoxesCommandHandlerTests
{
    private readonly Mock<IExcelReaderService> _excelReader;
    private readonly Mock<IBoxRepository> _boxRepository;
    private readonly Mock<IZoneRepository> _zoneRepository;
    private readonly ImportBoxesCommandHandler _handler;

    public ImportBoxesCommandHandlerTests()
    {
        _excelReader = new Mock<IExcelReaderService>();
        _boxRepository = new Mock<IBoxRepository>();
        _zoneRepository = new Mock<IZoneRepository>();
        _zoneRepository.Setup(r => r.GetByNameAsync("Sin especificar", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Zone("Sin especificar"));

        _handler = new ImportBoxesCommandHandler(
            _excelReader.Object,
            _boxRepository.Object,
            _zoneRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidFile_CreatesBoxesForEachSheet()
    {
        var zoneTaller = new Zone("Taller");
        _zoneRepository.Setup(r => r.GetByNameAsync("Taller", It.IsAny<CancellationToken>()))
            .ReturnsAsync(zoneTaller);

        _excelReader
            .Setup(r => r.Read(It.IsAny<byte[]>(), It.IsAny<ISheetEntityBuilder<Box>>()))
            .Returns((byte[] _, ISheetEntityBuilder<Box> b) => SimulateSheets(b,
                ("Caja1", "Caja de herramientas", "Taller", new[] { ("Martillo", "Martillo de 500g"), ("Destornillador", "Destornillador plano") }),
                ("Caja2", "Caja de cables", null, new[] { ("Cable HDMI", "2 metros") })));

        var createdBoxes = new List<Box>();
        _boxRepository.Setup(r => r.CreateAsync(It.IsAny<Box>(), It.IsAny<CancellationToken>()))
            .Callback<Box, CancellationToken>((b, _) => createdBoxes.Add(b))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new ImportBoxesCommand("dGVzdA=="), default);

        result.TotalSheets.Should().Be(2);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);

        createdBoxes.Should().HaveCount(2);
        createdBoxes[0].Name.Should().Be("Caja de herramientas");
        createdBoxes[0].ZoneId.Should().Be(zoneTaller.Id);
        createdBoxes[0].Items.Should().HaveCount(2);

        createdBoxes[1].Name.Should().Be("Caja de cables");
        createdBoxes[1].Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_SheetWithMissingName_ReturnsFailure()
    {
        _excelReader
            .Setup(r => r.Read(It.IsAny<byte[]>(), It.IsAny<ISheetEntityBuilder<Box>>()))
            .Returns((byte[] _, ISheetEntityBuilder<Box> b) => SimulateSheets(b,
                ("EmptyBox", null, null, Array.Empty<(string, string)>())));

        var result = await _handler.Handle(new ImportBoxesCommand("dGVzdA=="), default);

        result.TotalSheets.Should().Be(1);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(1);
        result.Results[0].Error.Should().Contain("Box name is missing");
    }

    [Fact]
    public async Task Handle_WithZoneName_UsesExistingZone()
    {
        var zone = new Zone("Almacén");
        _zoneRepository.Setup(r => r.GetByNameAsync("Almacén", It.IsAny<CancellationToken>()))
            .ReturnsAsync(zone);

        _excelReader
            .Setup(r => r.Read(It.IsAny<byte[]>(), It.IsAny<ISheetEntityBuilder<Box>>()))
            .Returns((byte[] _, ISheetEntityBuilder<Box> b) => SimulateSheets(b,
                ("Sheet1", "Caja Almacén", "Almacén", Array.Empty<(string, string)>())));

        Box? capturedBox = null;
        _boxRepository.Setup(r => r.CreateAsync(It.IsAny<Box>(), It.IsAny<CancellationToken>()))
            .Callback<Box, CancellationToken>((b, _) => capturedBox = b);

        await _handler.Handle(new ImportBoxesCommand("dGVzdA=="), default);

        capturedBox!.ZoneId.Should().Be(zone.Id);
    }

    [Fact]
    public async Task Handle_WhenZoneNotFound_UsesDefaultZone()
    {
        var defaultZone = new Zone("Sin especificar");
        _zoneRepository.Setup(r => r.GetByNameAsync("Sin especificar", It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultZone);

        _zoneRepository.Setup(r => r.GetByNameAsync("Zona inexistente", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Zone?)null);

        _excelReader
            .Setup(r => r.Read(It.IsAny<byte[]>(), It.IsAny<ISheetEntityBuilder<Box>>()))
            .Returns((byte[] _, ISheetEntityBuilder<Box> b) => SimulateSheets(b,
                ("Sheet1", "Caja Test", "Zona inexistente", Array.Empty<(string, string)>())));

        Box? capturedBox = null;
        _boxRepository.Setup(r => r.CreateAsync(It.IsAny<Box>(), It.IsAny<CancellationToken>()))
            .Callback<Box, CancellationToken>((b, _) => capturedBox = b);

        await _handler.Handle(new ImportBoxesCommand("dGVzdA=="), default);

        capturedBox!.ZoneId.Should().Be(defaultZone.Id);
    }

    [Fact]
    public async Task Handle_WithSomeValidAndSomeInvalid_CountsCorrectly()
    {
        _excelReader
            .Setup(r => r.Read(It.IsAny<byte[]>(), It.IsAny<ISheetEntityBuilder<Box>>()))
            .Returns((byte[] _, ISheetEntityBuilder<Box> b) => SimulateSheets(b,
                ("Hoja1", null, null, Array.Empty<(string, string)>()),
                ("Hoja2", "Caja Valida", null, Array.Empty<(string, string)>())));

        _boxRepository.Setup(r => r.CreateAsync(It.IsAny<Box>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new ImportBoxesCommand("dGVzdA=="), default);

        result.TotalSheets.Should().Be(2);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(1);
    }

    private static List<Box> SimulateSheets(
        ISheetEntityBuilder<Box> builder,
        params (string SheetName, string? BoxName, string? ZoneName, (string, string)[] Items)[] sheets)
    {
        var boxes = new List<Box>();

        foreach (var (sheetName, boxName, zoneName, items) in sheets)
        {
            builder.StartSheet(sheetName);

            if (boxName is not null)
            {
                builder.ReadCell(1, "B", boxName);
                if (zoneName is not null)
                    builder.ReadCell(2, "B", zoneName);
                foreach (var (itemName, itemDesc) in items)
                {
                    var row = 4 + Array.IndexOf(items, (itemName, itemDesc));
                    builder.ReadCell(row, "B", itemName);
                    builder.ReadCell(row, "C", itemDesc);
                }
            }

            var box = builder.Build();
            if (box is not null)
                boxes.Add(box);
        }

        return boxes;
    }
}
