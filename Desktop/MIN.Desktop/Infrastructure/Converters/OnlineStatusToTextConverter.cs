using System;
using System.Globalization;
using MIN.Core.Entities.Contracts.Enums;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для превращения статуса в текст
/// </summary>
public class OnlineStatusToTextConverter : Converter<OnlineStatusToTextConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not OnlineStatus onlineStatus)
        {
            return string.Empty;
        }

        var resultText = string.Empty;
        switch (onlineStatus)
        {
            case OnlineStatus.Online:
                resultText = "В сети";
                break;

            case OnlineStatus.Typing:
                resultText = "Печатает . . .";
                break;

            case OnlineStatus.Offline:
                resultText = "Последний раз в сети: ";
                break;
        }

        return resultText;
    }
}
