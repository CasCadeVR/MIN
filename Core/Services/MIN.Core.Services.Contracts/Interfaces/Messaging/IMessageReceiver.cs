using MIN.Core.Messaging.Contracts.Events;

namespace MIN.Core.Services.Contracts.Interfaces.Messaging;

/// <summary>
/// Сервис по получению и приёму сообщений
/// </summary>
public interface IMessageReceiver
{
    /// <summary>
    /// Начать прослушку сообщений
    /// </summary>
    Task StartListeningAsync(CancellationToken cancellationToken = default);
}
