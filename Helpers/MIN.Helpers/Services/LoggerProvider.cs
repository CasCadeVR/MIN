using System.Text;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Helpers.Services;

/// <inheritdoc cref="ILoggerProvider"/>
public class LoggerProvider : ILoggerProvider
{
    private readonly List<LogItem> messages = [];

    ///<inheritdoc cref="ILoggerProvider.OnLogReceived"/>
    public event EventHandler<LogItem>? OnLogReceived;

    void ILoggerProvider.Log(string message, LogLevel level)
    {
        var formatted = new StringBuilder();
        formatted.Append(DateTime.Now.ToString("HH:mm:ss.fff"));
        formatted.Append(" - ");
        formatted.Append(Enum.GetName(level));
        formatted.Append(" - ");
        formatted.Append(message);
        var result = formatted.ToString();

        var item = new LogItem(result, level);

        messages.Add(item);
        OnLogReceived?.Invoke(this, item);
    }

    IEnumerable<LogItem> ILoggerProvider.GetRecentLogHistory(int? page, int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            return messages.AsEnumerable().Reverse().Skip(page.Value * pageSize.Value).Take(pageSize.Value);
        }
        else
        {
            return messages;
        }
    }
}
