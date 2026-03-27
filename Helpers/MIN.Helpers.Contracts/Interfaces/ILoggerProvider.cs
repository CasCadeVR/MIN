using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Helpers.Contracts.Interfaces;

/// <summary>
/// Сервис по работе с логами
/// </summary>
public interface ILoggerProvider
{
    /// <summary>
    /// Событие на получение лога
    /// </summary>
    event EventHandler<string>? OnLogReceived;

    /// <summary>
    /// Залогировать
    /// </summary>
    void Log(string message, LogLevel level = LogLevel.Information);

    /// <summary>
    /// Получить историю логов
    /// </summary>
    IEnumerable<string> GetLogHistory();
}
