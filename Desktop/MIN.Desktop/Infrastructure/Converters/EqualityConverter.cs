using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для сравнения значений
/// </summary>
public class EqualityConverter : Converter<EqualityConverter>
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => Equals(value, parameter);
}
