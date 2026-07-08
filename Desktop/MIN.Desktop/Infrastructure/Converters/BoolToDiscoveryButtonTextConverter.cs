using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для выделения карточки
/// </summary>
public class BoolToDiscoveryButtonTextConverter : Converter<BoolToDiscoveryButtonTextConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool IsDiscovering)
        {
            return "Всего нашлось комнат: ";
        }

        return IsDiscovering ? "Остановить поиск" : "Найти комнаты";
    }
}
