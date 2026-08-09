namespace MIN.Common.Core.Extensions;

/// <summary>
/// Методы расширения для времени
/// </summary>
public static class TimeSpanExtensions
{
    /// <summary>
    /// Перевести продолжительность в приятную для человека строку
    /// </summary>
    public static string ToFriendlyString(this TimeSpan duration) => FormatDuration(duration);

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalSeconds < 1)
        {
            return "менее секунды";
        }

        if (duration.TotalDays >= 1)
        {
            var days = (int)duration.TotalDays;
            return $"{days} {Decline(days, "день", "дня", "дней")}";
        }

        if (duration.TotalHours >= 1)
        {
            var hours = (int)duration.TotalHours;
            return $"{hours} {Decline(hours, "час", "часа", "часов")}";
        }

        if (duration.TotalMinutes >= 1)
        {
            var minutes = (int)duration.TotalMinutes;
            return $"{minutes} {Decline(minutes, "минута", "минуты", "минут")}";
        }

        var seconds = (int)duration.TotalSeconds;
        return $"{seconds} {Decline(seconds, "секунда", "секунды", "секунд")}";
    }

    private static string Decline(int number, string form1, string form2, string form5)
    {
        var n = number % 100;
        if (n >= 11 && n <= 19)
        {
            return form5;
        }

        var m = number % 10;
        if (m == 1)
        {
            return form1;
        }

        return m >= 2 && m <= 4 ? form2 : form5;
    }
}

