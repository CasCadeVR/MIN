using System;
using System.Globalization;
using Avalonia.Layout;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Форматирует связанное значение как строку из целого числа
/// </summary>
public partial class IsSelfBoolHorizontalToConverter : Converter<IsSelfBoolHorizontalToConverter>
{
    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        return HorizontalAlignment.Center;
    }
}
