using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для скрытия счётчика, если он равен 0
/// </summary>
public class GreaterThanZeroConverter : Converter<GreaterThanZeroConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is int count && count > 0;
    }
}
