using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для сравнения значений, которые не должны быть null
/// </summary>
public class NotNullConverter : Converter<NotNullConverter>
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not null;
}
