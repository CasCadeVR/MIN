using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Форматирует связанное значение как строку из целого числа
/// </summary>
public partial class IntToStringConverter : Converter<IntToStringConverter>
{
    [GeneratedRegex("[^0-9]")]
    private static partial Regex DigitReplaceRegex();

    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value?.ToString() ?? "";

    /// <inheritdoc />
    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return 0;
        }

        if (value is not string str)
        {
            str = value?.ToString() ?? string.Empty;
            if (str is null || str == string.Empty)
            {
                return 0;
            }
        }

        str = DigitReplaceRegex().Replace(str, "");
        if (int.TryParse(str, out var result))
        {
            return result;
        }
        return 0;
    }
}
