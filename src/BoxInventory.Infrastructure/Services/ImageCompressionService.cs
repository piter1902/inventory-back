using BoxInventory.Application.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace BoxInventory.Infrastructure.Services;

public class ImageCompressionService : IImageCompressionService
{
    public string? Compress(string? imageBase64, int maxWidth = 1920, int maxHeight = 1080, int quality = 75)
    {
        if (string.IsNullOrEmpty(imageBase64))
            return imageBase64;

        try
        {
            var dataIndex = imageBase64.IndexOf("base64,", StringComparison.Ordinal);
            var base64Data = dataIndex >= 0
                ? imageBase64[(dataIndex + 7)..]
                : imageBase64;

            var imageBytes = Convert.FromBase64String(base64Data);

            using var image = Image.Load(imageBytes);

            if (image.Width > maxWidth || image.Height > maxHeight)
            {
                var ratio = Math.Min((double)maxWidth / image.Width, (double)maxHeight / image.Height);
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);
                image.Mutate(x => x.Resize(newWidth, newHeight));
            }

            using var ms = new MemoryStream();
            var encoder = new JpegEncoder { Quality = quality };
            image.Save(ms, encoder);
            var compressed = Convert.ToBase64String(ms.ToArray());

            return $"data:image/jpeg;base64,{compressed}";
        }
        catch
        {
            return imageBase64;
        }
    }
}
