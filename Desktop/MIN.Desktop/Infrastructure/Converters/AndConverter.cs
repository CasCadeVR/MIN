using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для оператора И
/// </summary>
public class AndMultiConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.OfType<bool>().All(v => v);
    }
}
