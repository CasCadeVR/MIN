using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Helpers.Contracts.Models;

/// <summary>
/// Модель строчки лога
/// </summary>
public class LogItem
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Уровень серъёзности сообщения в логах
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="LogItem"/>
    /// </summary>
    public LogItem(string message, LogLevel logLevel = LogLevel.Information)
    {
        Message = message;
        LogLevel = logLevel;
    }
}
