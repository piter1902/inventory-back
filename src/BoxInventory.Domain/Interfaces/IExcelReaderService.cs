namespace BoxInventory.Domain.Interfaces;

public interface IExcelReaderService
{
    List<T> Read<T>(byte[] fileBytes, ISheetEntityBuilder<T> builder) where T : class;
}
