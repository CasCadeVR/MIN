using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для сравнения значений
/// </summary>
public class NonEqualityConverter : Converter<NonEqualityConverter>
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => !Equals(value, parameter);
}
