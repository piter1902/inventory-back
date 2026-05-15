using BoxInventory.Application.Boxes.Queries.Search;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;

namespace BoxInventory.Tests.Application.Handlers;

public class SearchQueryHandlerTests
{
    private readonly Mock<IBoxRepository> _repository;
    private readonly SearchQueryHandler _handler;

    public SearchQueryHandlerTests()
    {
        _repository = new Mock<IBoxRepository>();
        _handler = new SearchQueryHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_EmptyQuery_ReturnsEmpty()
    {
        var result = await _handler.Handle(new SearchQuery(""), default);

        result.Boxes.Should().BeEmpty();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_NullQuery_ReturnsEmpty()
    {
        var result = await _handler.Handle(new SearchQuery(null!), default);

        result.Boxes.Should().BeEmpty();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MatchingBoxName_ReturnsBox()
    {
        var boxes = new List<Box>
        {
            new("BOX-001", "Electronics Box", null),
            new("BOX-002", "Kitchen Box", null),
        };
        _repository.Setup(r => r.SearchByNameAsync("Electronics", It.IsAny<CancellationToken>()))
            .ReturnsAsync(boxes.Where(b => b.Name.Contains("Electronics")).ToList());

        var result = await _handler.Handle(new SearchQuery("Electronics"), default);

        result.Boxes.Should().ContainSingle(b => b.Name == "Electronics Box");
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MatchingItemName_ReturnsItemWithBoxInfo()
    {
        var box = new Box("BOX-001", "Caja Herramientas", null);
        box.AddItem(new Item("Martillo", "Martillo de 500g"));
        box.AddItem(new Item("Destornillador", "Destornillador plano"));

        var otherBox = new Box("BOX-002", "Caja Cocina", null);
        otherBox.AddItem(new Item("Cuchillo", "Cuchillo de chef"));

        _repository.Setup(r => r.SearchByNameAsync("Martillo", It.IsAny<CancellationToken>()))
            .ReturnsAsync([box]);

        var result = await _handler.Handle(new SearchQuery("Martillo"), default);

        result.Boxes.Should().BeEmpty();
        result.Items.Should().ContainSingle();
        result.Items[0].Name.Should().Be("Martillo");
        result.Items[0].BoxName.Should().Be("Caja Herramientas");
        result.Items[0].BoxId.Should().Be(box.Id.ToString());
    }

    [Fact]
    public async Task Handle_MatchBothBoxAndItem_ReturnsSegmented()
    {
        var box = new Box("BOX-001", "Electro Box", null);
        box.AddItem(new Item("Electro Cable", "Cable HDMI 2m"));

        var otherBox = new Box("BOX-002", "Cocina", null);
        otherBox.AddItem(new Item("Cuchillo", "Acero"));

        _repository.Setup(r => r.SearchByNameAsync("Electro", It.IsAny<CancellationToken>()))
            .ReturnsAsync([box]);

        var result = await _handler.Handle(new SearchQuery("Electro"), default);

        result.Boxes.Should().Contain(b => b.Name == "Electro Box");
        result.Items.Should().Contain(i => i.Name == "Electro Cable");
    }

    [Fact]
    public async Task Handle_NoMatch_ReturnsEmpty()
    {
        _repository.Setup(r => r.SearchByNameAsync("xyz123", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(new SearchQuery("xyz123"), default);

        result.Boxes.Should().BeEmpty();
        result.Items.Should().BeEmpty();
    }
}
