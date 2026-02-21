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
        private List<string> messages = new List<string>();

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
            messages.Add(message);
            OnLogRecieved?.Invoke(this, message);
        }

        IEnumerable<string> ILoggerProvider.GetLogHistory()
            => messages;
    }
}
