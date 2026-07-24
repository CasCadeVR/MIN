using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для выделения карточки
/// </summary>
public class LogLevelToBrushConverter : Converter<LogLevelToBrushConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not LogLevel logLevel)
        {
            return Brushes.Transparent;
        }

        var key = logLevel switch
        {
            LogLevel.Information => "StatusInfo",
            LogLevel.Warning => "StatusWarning",
            LogLevel.Error => "StatusError",
            _ => throw new NotSupportedException()
        };

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
