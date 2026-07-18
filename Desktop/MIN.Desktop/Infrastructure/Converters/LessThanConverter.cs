using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для сравнения чисел
/// </summary>
public class LessThanConverter : Converter<LessThanConverter>
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IComparable comparable || parameter is null)
        {
            return false;
        }

        return parameter is IComparable paramComparable && comparable.CompareTo(paramComparable) < 0;
    }
}
