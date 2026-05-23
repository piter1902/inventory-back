using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MongoDB.Bson;

namespace BoxInventory.Application.Boxes.Commands.ImportBoxes;

public class BoxSheetBuilder : ISheetEntityBuilder<Box>
{
    private string _currentSheet = "";
    private string? _boxName;
    private string? _zoneName;
    private readonly List<(string Name, string? Description)> _items = new();

    private readonly List<SheetResult> _results = new();

    public IReadOnlyList<SheetResult> Results => _results;

    public void StartSheet(string sheetName)
    {
        _currentSheet = sheetName;
        _boxName = null;
        _zoneName = null;
        _items.Clear();
    }

    public void ReadCell(int row, string column, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (row == 1 && column == "B")
            _boxName = value;
        else if (row == 2 && column == "B")
            _zoneName = value;
        else if (row >= 4 && column == "B")
            _items.Add((value, null));
        else if (row >= 4 && column == "C" && _items.Count > 0)
            _items[^1] = (_items[^1].Name, value);
    }

    public Box? Build()
    {
        if (string.IsNullOrWhiteSpace(_boxName))
        {
            _results.Add(new SheetResult(_currentSheet, null, "Box name is missing or invalid in cell B1"));
            return null;
        }

        var identifier = $"BOX-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var box = new Box(identifier, _boxName, null, null, ObjectId.Empty);

        foreach (var (name, desc) in _items)
        {
            if (!string.IsNullOrWhiteSpace(name))
                box.AddItem(new Item(name, desc ?? string.Empty));
        }

        _results.Add(new SheetResult(_currentSheet, _zoneName, null));
        return box;
    }

    public record SheetResult(string SheetName, string? ZoneName, string? Error);
}
