using System;
using System.Globalization;

namespace MIN.Desktop.Infrastructure.Converters;

/// <summary>
/// Режим форматирования времени
/// </summary>
internal enum DateMode
{
    /// <summary>
    /// Относительно от текущего времени
    /// </summary>
    Relative,

    /// <summary>
    /// Короткая дата
    /// </summary>
    Short
}

/// <summary>
/// Форматирует связанное значение в виде относительной строки даты, полученной из значения типа DateTime.
/// </summary>
internal sealed class DateConverter : Converter<DateConverter>
{
    private const float DAYS_IN_YEAR = 365.2425f;
    private const float MEAN_DAYS_IN_MONTH = DAYS_IN_YEAR / 12f;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var mode = parameter is DateMode dm ? dm : DateMode.Relative;

        return value switch
        {
            DateTime dateTime => Format(dateTime, mode),
            DateTimeOffset dateTimeOffset => Format(dateTimeOffset.DateTime, mode),
            DateOnly dateOnly => Format(dateOnly.ToDateTime(TimeOnly.MinValue), mode),
            string text when DateTimeOffset.TryParse(text, out var offset) => Format(offset.DateTime, mode),
            _ => throw new ArgumentException($"Value must be DateTime, DateTimeOffset, DateOnly, or parsable string", nameof(value))
        };
    }

    private string Format(DateTime date, DateMode mode)
    {
        var delta = DateTimeOffset.UtcNow - date;

        return mode switch
        {
            DateMode.Short => FormatShort(date, delta),
            _ => FormatRelative(delta)
        };
    }

    private string FormatShort(DateTime date, TimeSpan delta)
    {
        return delta switch
        {
            { TotalDays: < 1 } => $"{date:t}",
            { TotalDays: < 2 } => $"{date.DayOfWeek}",
            { TotalDays: < MEAN_DAYS_IN_MONTH } => $"{date:d}",
            _ => $"{date:D}"
        };
    }

    private string FormatRelative(TimeSpan delta)
    {
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
