using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для выделения числа пинга
/// </summary>
public class PingToBrushConverter : Converter<PingToBrushConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int ping)
        {
            return Brushes.Transparent;
        }

        string? key;

        if (ping >= 0 && ping < 300)
        {
            key = "StatusSuccess";
        }
        else if (ping >= 300 && ping < 1000)
        {
            key = "StatusWarning";
        }
        else
        {
            key = "StatusError";
        }

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
