using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для выделения карточки
/// </summary>
public class BoolToDiscoveryCounterConverter : Converter<BoolToDiscoveryCounterConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool IsDiscovering)
        {
            return "Всего нашлось комнат: ";
        }

        return IsDiscovering ? "Поиск комнат..." : "Всего нашлось комнат: ";
    }
}
