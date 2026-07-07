using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MIN.Desktop.Infrastructure.Converters.RecentRoom;

/// <summary>
/// Конвертор для выделения карточки
/// </summary>
public class BoolToBrushConverter : Converter<BoolToBrushConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isSelected)
        {
            return Brushes.Transparent;
        }

        var key = isSelected ? "Primary" : "SurfaceCard";
        var app = Application.Current;
        var theme = app?.ActualThemeVariant;
        if (theme != null && app?.TryFindResource(key, theme, out var res) == true)
        {
            return res switch
            {
                Brush brush => brush,
                Color color => new SolidColorBrush(color),
                _ => Brushes.Transparent
            };
        }
        return Brushes.Transparent;
    }
}
