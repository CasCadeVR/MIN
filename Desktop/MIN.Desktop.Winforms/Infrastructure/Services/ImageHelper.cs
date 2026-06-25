using System.Drawing.Drawing2D;

namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Помощник с выгрузкой изображений из файлов
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// Получить размеры изображения из файла
    /// </summary>
    public static Size GetDimensions(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var img = Image.FromStream(fs, false, false);
        return new Size(img.Width, img.Height);
    }

    /// <summary>
    /// Загрузить изобржание, урезав его по максимальной ширине и сохранив отношение
    /// </summary>
    public static Image LoadScaled(string filePath, int maxWidth)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var original = Image.FromStream(fs, false, false);

        var ratio = (double)original.Width / original.Height;
        var width = Math.Min(maxWidth, original.Width);
        var height = (int)(width / ratio);

        var bitmap = new Bitmap(width, height);
        using var g = Graphics.FromImage(bitmap);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.DrawImage(original, 0, 0, width, height);

        return bitmap;
    }
}
