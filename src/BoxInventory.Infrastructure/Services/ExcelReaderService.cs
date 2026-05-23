using BoxInventory.Domain.Interfaces;
using ClosedXML.Excel;

namespace BoxInventory.Infrastructure.Services;

public class ExcelReaderService : IExcelReaderService
{
    public List<T> Read<T>(byte[] fileBytes, ISheetEntityBuilder<T> builder) where T : class
    {
        using var stream = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(stream);

        var results = new List<T>();

        foreach (var worksheet in workbook.Worksheets)
        {
            builder.StartSheet(worksheet.Name);

            foreach (var row in worksheet.Rows())
            {
                foreach (var cell in row.Cells())
                {
                    var value = cell.GetString().Trim();
                    builder.ReadCell(row.RowNumber(), cell.Address.ColumnLetter, value);
                }
            }

            var entity = builder.Build();
            if (entity is not null)
                results.Add(entity);
        }

        return results;
    }
}
