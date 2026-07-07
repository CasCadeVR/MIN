using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters.RecentRoom;

/// <summary>
/// Форматирует связанное значение в виде сокращённой даты, полученной из значения типа DateTime.
/// </summary>
internal sealed class DateToShortDateConverter : Converter<DateToShortDateConverter>
{
    private const float DAYS_IN_YEAR = 365.2425f;
    private const float MEAN_DAYS_IN_MONTH = DAYS_IN_YEAR / 12f;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        DateTime date = value switch
        {
            DateTime dateTime => dateTime,
            DateTimeOffset dateTimeOffset => dateTimeOffset.DateTime,
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            string text when DateTimeOffset.TryParse(text, out DateTimeOffset offset) => offset.DateTime,
            _ => throw new ArgumentException($"Value must be a {nameof(DateTime)} or {nameof(DateTimeOffset)}", nameof(value))
        };

        TimeSpan delta = DateTimeOffset.UtcNow - date;

        return delta switch
        {
            { TotalDays: < 1 } => $"{date:t}",
            { TotalDays: < 2 } => $"{date.DayOfWeek}",
            { TotalDays: < MEAN_DAYS_IN_MONTH } => $"{date:d}",
            _ => $"{date:D}"
        };
    }
}
