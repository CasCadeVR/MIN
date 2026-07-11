using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для выделения карточки в качесвте приватного собеседника
/// </summary>
public class BoolToPrivateChatTextConverter : Converter<BoolToPrivateChatTextConverter>
{
    private const string StartPrivateChatText = "Начать приватное общение";
    private const string StopPrivateChatText = "Прекратить приватное общение";

    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool IsSelected)
        {
            return StartPrivateChatText;
        }

        return IsSelected ? StopPrivateChatText : StartPrivateChatText;
    }
}
