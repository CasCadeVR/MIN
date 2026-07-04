using System;
using System.Collections.Generic;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// При получении значения программным способом происходит обрезка, но пробелы в поле ввода сохраняются для улучшения пользовательского опыта
/// </summary>
/// <remarks>
///     Этот конвертер является нетрадиционным (инвертированный конвертер), поскольку значение преобразуется для бэкэнда
///     Пользователь хочет иметь возможность вводить пробелы во время набора текста, но мы не хотим сохранять эти пробелы
/// </remarks>
public class TrimConverter : Converter<TrimConverter>
{
    private readonly object inOutCacheLock = new();

    private readonly Dictionary<string, string> inOutCache = new();

    /// <summary>
    /// Converts trimmed value back to last known untrimmed value.
    /// </summary>
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

    /// <summary>
    /// Converts untrimmed value back to trimmed value.
    /// </summary>
    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string strValue)
        {
            return value ?? string.Empty;
        }
        if (!strValue.StartsWith(' ') && !strValue.EndsWith(' '))
        {
            // It's safe to reset cache now.
            lock (inOutCacheLock)
            {
                inOutCache.Clear();
            }
            return strValue;
        }
        string trim = strValue.Trim();
        lock (inOutCacheLock)
        {
            inOutCache[trim] = strValue;
        }
        return trim;
    }
}
