using BoxInventory.Application.Boxes.Queries.GetMovementLogs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MongoDB.Bson;

namespace BoxInventory.Tests.Application.Queries;

public class GetMovementLogsQueryHandlerTests
{
    private readonly Mock<IItemMovementLogRepository> _logRepository;
    private readonly GetMovementLogsQueryHandler _handler;

    public GetMovementLogsQueryHandlerTests()
    {
        _logRepository = new Mock<IItemMovementLogRepository>();
        _handler = new GetMovementLogsQueryHandler(_logRepository.Object);
    }

    [Fact]
    public async Task Handle_WithoutBoxId_ReturnsAllLogs()
    {
        var logs = new List<ItemMovementLog>
        {
            CreateLog("item1", "Martillo", "box1", "Caja A", "box2", "Caja B", "juan"),
            CreateLog("item2", "Destornillador", "box2", "Caja B", "box3", "Caja C", "maria"),
        };

        _logRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var result = await _handler.Handle(new GetMovementLogsQuery(null), default);

        result.Should().HaveCount(2);
        result[0].ItemName.Should().Be("Martillo");
        result[1].ItemName.Should().Be("Destornillador");
    }

    [Fact]
    public async Task Handle_WithBoxId_FiltersByBox()
    {
        var logs = new List<ItemMovementLog>
        {
            CreateLog("item1", "Martillo", "box1", "Caja A", "box2", "Caja B", "juan"),
        };

        _logRepository.Setup(r => r.GetByBoxIdAsync("box1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var result = await _handler.Handle(new GetMovementLogsQuery("box1"), default);

        result.Should().HaveCount(1);
        result[0].SourceBoxId.Should().Be("box1");
    }

    [Fact]
    public async Task Handle_NoLogs_ReturnsEmptyList()
    {
        _logRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemMovementLog>());

        var result = await _handler.Handle(new GetMovementLogsQuery(null), default);

        result.Should().BeEmpty();
    }

    private static ItemMovementLog CreateLog(
        string itemId, string itemName,
        string sourceBoxId, string sourceBoxName,
        string destBoxId, string destBoxName,
        string movedBy)
    {
        return new ItemMovementLog(itemId, itemName, sourceBoxId, sourceBoxName, destBoxId, destBoxName, movedBy);
    }
}
