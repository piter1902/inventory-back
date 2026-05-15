using BoxInventory.Application.Boxes.Commands.CreateBox;
using BoxInventory.Application.Common.Interfaces;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MongoDB.Bson;

namespace BoxInventory.Tests.Application.Handlers;

public class CreateBoxCommandHandlerTests
{
    private readonly Mock<IBoxRepository> _repository;
    private readonly Mock<IZoneRepository> _zoneRepository;
    private readonly Mock<IImageCompressionService> _imageCompression;
    private readonly CreateBoxCommandHandler _handler;

    public CreateBoxCommandHandlerTests()
    {
        _repository = new Mock<IBoxRepository>();
        _zoneRepository = new Mock<IZoneRepository>();
        _zoneRepository.Setup(r => r.GetByNameAsync("Sin especificar", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Zone("Sin especificar"));
        _imageCompression = new Mock<IImageCompressionService>();
        _imageCompression.Setup(c => c.Compress(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((string? s, int _, int __, int ___) => s);
        _handler = new CreateBoxCommandHandler(_repository.Object, _zoneRepository.Object, _imageCompression.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesBox()
    {
        Box? capturedBox = null;
        _repository.Setup(r => r.CreateAsync(It.IsAny<Box>(), It.IsAny<CancellationToken>()))
            .Callback<Box, CancellationToken>((b, _) => capturedBox = b);

        var result = await _handler.Handle(new CreateBoxCommand("Caja", null, null, null, null), default);

        result.Name.Should().Be("Caja");
        result.Identifier.Should().NotBeEmpty();
        result.Identifier.Should().StartWith("BOX-");
        result.QrUrl.Should().Be($"/box/{result.Identifier}");
        capturedBox.Should().NotBeNull();
        capturedBox!.Identifier.Should().Be(result.Identifier);
    }

    [Fact]
    public async Task Handle_WithoutName_CreatesBox()
    {
        _repository.Setup(r => r.CreateAsync(It.IsAny<Box>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new CreateBoxCommand(null, null, null, null, null), default);

        result.Name.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithItems_CreatesBoxWithItems()
    {
        var items = new List<CreateItemRequest>
        {
            new("Cable HDMI", "Cable negro"),
            new("Cable USB", null),
        };

        var result = await _handler.Handle(new CreateBoxCommand("Caja", null, null, null, items), default);

        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(i => i.Name == "Cable HDMI" && i.Description == "Cable negro");
        result.Items.Should().Contain(i => i.Name == "Cable USB" && i.Description == "");
    }

    [Fact]
    public async Task Handle_WithoutItems_ReturnsEmptyItems()
    {
        var result = await _handler.Handle(new CreateBoxCommand("Caja", null, null, null, null), default);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GeneratesDifferentIdentifiers()
    {
        var result1 = await _handler.Handle(new CreateBoxCommand(null, null, null, null, null), default);
        var result2 = await _handler.Handle(new CreateBoxCommand(null, null, null, null, null), default);

        result1.Identifier.Should().NotBe(result2.Identifier);
    }
}
