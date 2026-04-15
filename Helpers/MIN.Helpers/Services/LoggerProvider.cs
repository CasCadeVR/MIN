using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;
using System.Text;

namespace MIN.Helpers.Services
{
    /// <summary>
    /// <inheritdoc cref="ILoggerProvider"/>
    /// </summary>
    public class LoggerProvider : ILoggerProvider
    {
        private readonly List<string> messages = [];

        ///<inheritdoc cref="ILoggerProvider.OnLogReceived"/>
        public event EventHandler<string>? OnLogReceived;

        void ILoggerProvider.Log(string message, LogLevel level)
        {
            var formatted = new StringBuilder();
            formatted.Append(DateTime.Now.ToString("HH:mm:ss.fff"));
            formatted.Append(" - ");
            formatted.Append(Enum.GetName(level));
            formatted.Append(" - ");
            formatted.Append(message);
            var result = formatted.ToString();
            messages.Add(result);
            OnLogReceived?.Invoke(this, result);
        }

        IEnumerable<string> ILoggerProvider.GetLogHistory()
            => messages;
    }
}
