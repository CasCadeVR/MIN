using System;
using System.Collections.Generic;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// При получении значения программным способом происходит обрезка, но пробелы в поле ввода сохраняются для улучшения пользовательского опыта
/// </summary>
/// <remarks>
/// Этот конвертер является нетрадиционным (инвертированный конвертер), поскольку значение преобразуется для бэкэнда
/// Пользователь хочет иметь возможность вводить пробелы во время набора текста, но мы не хотим сохранять эти пробелы
/// </remarks>
public class TrimConverter : Converter<TrimConverter>
{
    private readonly Dictionary<string, string> inOutCache = [];
    private readonly object inOutCacheLock = new();

    /// <inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string strValue)
        {
            return value ?? string.Empty;
        }
        lock (inOutCacheLock)
        {
            if (inOutCache.TryGetValue(strValue.Trim(), out var untrimmedValue))
            {
                strValue = untrimmedValue;
            }
        }
        return strValue;
    }

    /// <inheritdoc />
    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string strValue)
        {
            return value ?? string.Empty;
        }
        if (strValue.Length == 0 || (!char.IsWhiteSpace(strValue[0]) && !char.IsWhiteSpace(strValue[^1])))
        {
            // It's safe to reset cache now.
            lock (inOutCacheLock)
            {
                inOutCache.Clear();
            }
            return strValue;
        }
        var trim = strValue.Trim();
        if (trim.Length > 0)
        {
            lock (inOutCacheLock)
            {
                inOutCache[trim] = strValue;
            }
        }
        return trim;
    }
}
