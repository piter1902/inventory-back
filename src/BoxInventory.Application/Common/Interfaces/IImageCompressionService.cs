namespace BoxInventory.Application.Common.Interfaces;

public interface IImageCompressionService
{
    string? Compress(string? imageBase64, int maxWidth = 1920, int maxHeight = 1080, int quality = 75);
}
