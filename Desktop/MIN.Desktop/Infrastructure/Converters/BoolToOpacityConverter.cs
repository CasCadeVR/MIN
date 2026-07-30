using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Преобразует значение переменной в тип double из логического значения
/// Если значение истинно, возвращает 1.0. Если ложно, возвращает 0.0
/// </summary>
public class BoolToOpacityConverter : Converter<BoolToOpacityConverter>
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? 1.0 : 0.0;
        }
        return 0.0;
    }
}
