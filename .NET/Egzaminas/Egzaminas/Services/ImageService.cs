using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Egzaminas.Services.Interfaces;

namespace Egzaminas.Services;

public class ImageService : IImageService
{
    public async Task<byte[]> ProcessProfilePicture(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new InvalidDataException("Profile picture is required");
        }

        if (!IsImageFile(file))
        {
            throw new InvalidDataException("File must an image (jpg, png, gif, bmp)");
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using (var image = Image.FromStream(memoryStream))
            {
                var resizedImage = ResizeImage(image, 200, 200);

                using (var outputStream = new MemoryStream())
                {
                    resizedImage.Save(outputStream, ImageFormat.Jpeg);
                    return outputStream.ToArray();
                }
            }
        }
    }

    private bool IsImageFile(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(fileExtension);
    }

    private Image ResizeImage(Image image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);
        var destImage = new Bitmap(width, height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }
}
