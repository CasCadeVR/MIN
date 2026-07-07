using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Форматирует связанное значение в виде относительной строки даты, полученной из значения типа DateTime.
/// </summary>
internal sealed class DateToRelativeDateConverter : Converter<DateToRelativeDateConverter>
{
    private const float DAYS_IN_YEAR = 365.2425f;
    private const float MEAN_DAYS_IN_MONTH = DAYS_IN_YEAR / 12f;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        DateTimeOffset date = value switch
        {
            DateTime dateTime => dateTime,
            DateTimeOffset dateTimeOffset => dateTimeOffset,
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            string text when DateTimeOffset.TryParse(text, out DateTimeOffset offset) => offset,
            _ => throw new ArgumentException($"Value must be a {nameof(DateTime)} or {nameof(DateTimeOffset)}", nameof(value))
        };

        TimeSpan delta = DateTimeOffset.UtcNow - date.UtcDateTime;

        return delta switch
        {
            { TotalSeconds: < 1 } => "сейчас",
            { TotalSeconds: < 2 } => "секунду назад",
            { TotalMinutes: < 1 } => $"{(int)delta.TotalSeconds} секунд назад",
            { TotalMinutes: < 2 } => "минуту назад",
            { TotalMinutes: < 45 } => $"{(int)delta.TotalMinutes} минут назад",
            { TotalHours: < 1.5 } => "час назад",
            { TotalDays: < 1 } => $"{(int)delta.TotalHours} часов назад",
            { TotalDays: < 2 } => "вчера",
            { TotalDays: < MEAN_DAYS_IN_MONTH } => $"{(int)delta.TotalDays} дней назад",
            { TotalDays: < MEAN_DAYS_IN_MONTH * 2 } => "месяц назад",
            { TotalDays: < DAYS_IN_YEAR } => $"{(int)(delta.TotalDays / MEAN_DAYS_IN_MONTH)} месяцев назад",
            { TotalDays: < DAYS_IN_YEAR * 2 } => "год назад",
            _ => $"{(int)(delta.TotalDays / DAYS_IN_YEAR)} лет назад"
        };
    }
}
