using System;
using System.Globalization;
using Avalonia.Layout;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Конвертор для выравнивания карточки в зависимости от отправителя
/// </summary>
public class BoolToMessageAlignConverter : Converter<BoolToMessageAlignConverter>
{
    ///<inheritdoc />
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool IsLocal)
        {
            return HorizontalAlignment.Center;
        }

        return IsLocal ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }
}
