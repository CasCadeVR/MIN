using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models.Enums;
using System.Text;

namespace MIN.Services.Services
{
    /// <summary>
    /// <inheritdoc cref="ILoggerProvider"/>
    /// </summary>
    public class LoggerProvider : ILoggerProvider
    {
        private List<string> messages = [];

        ///<inheritdoc cref="ILoggerProvider.OnLogRecieved"/>
        public event EventHandler<string>? OnLogRecieved;

        void ILoggerProvider.Log(string message, LogLevel level)
        {
            var formatted = new StringBuilder();
            formatted.Append(TimeOnly.FromDateTime(DateTime.Now).ToShortTimeString());
            formatted.Append(" - ");
            formatted.Append(Enum.GetName(level));
            formatted.Append(" - ");
            formatted.Append(message);
            var result = formatted.ToString();
            messages.Add(result);
            OnLogRecieved?.Invoke(this, result);
        }

        IEnumerable<string> ILoggerProvider.GetLogHistory()
            => messages;
    }
}
