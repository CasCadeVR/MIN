using Avalonia.Media.Imaging;
using SkiaSharp;
using Svg.Skia;

namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Помошник с изображениями
/// </summary>
public class ImageHelper
{
    private const int SvgDefaultResolution = 100;

    /// <summary>
    /// Перевести svg в Bitmap
    /// </summary>
    public static Bitmap SvgToBitmap(string svgPath, int width = SvgDefaultResolution, int height = SvgDefaultResolution)
    {
        var svg = new SKSvg();
        var svgBounds = svg.Load(svgPath);
        var matrix = SKMatrix.CreateScale(
            width / svgBounds!.CullRect.Width,
            height / svgBounds.CullRect.Height);

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.DrawPicture(svg.Picture, in matrix);
        var image = surface.Snapshot();
        var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return new Bitmap(data.AsStream());
    }
}
