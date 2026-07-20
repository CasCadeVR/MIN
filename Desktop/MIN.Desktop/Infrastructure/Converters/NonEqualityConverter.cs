using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для сравнения значений
/// </summary>
public class NonEqualityConverter : Converter<NonEqualityConverter>, IMultiValueConverter
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => !Equals(value, parameter);

    /// <inheritdoc />
    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

    /// <inheritdoc />
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        foreach (var val1 in values)
        {
            foreach (var val2 in values)
            {
                if (!ReferenceEquals(val1, val2))
                {
                    continue;
                }
                if (!Equals(val1, val2))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
