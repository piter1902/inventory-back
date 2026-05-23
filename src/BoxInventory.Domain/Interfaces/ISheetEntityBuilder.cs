namespace BoxInventory.Domain.Interfaces;

public interface ISheetEntityBuilder<T> where T : class
{
    void StartSheet(string sheetName);
    void ReadCell(int row, string column, string? value);
    T? Build();
}
