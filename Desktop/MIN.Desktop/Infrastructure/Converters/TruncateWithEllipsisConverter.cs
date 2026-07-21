using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для обрезания слишком большого текста
/// </summary>
public class TruncateWithEllipsisConverter : Converter<TruncateWithEllipsisConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text)
        {
            return string.Empty;
        }

        var maxLength = parameter is string s && int.TryParse(s, out var n) ? n : 35;
        return text.Length <= maxLength ? text : text[..maxLength] + "...";
    }
}
