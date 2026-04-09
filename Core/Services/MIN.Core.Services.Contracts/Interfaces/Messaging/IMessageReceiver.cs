using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;

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

    /// <summary>
    /// Закончить прослушку сообщений
    /// </summary>
    Task StopListeningAsync();

    /// <summary>
    /// Обработать сообщение вручную
    /// </summary>
    Task ReceiveAsLocal(IMessage message, ParticipantInfo sender, Guid roomId, Guid connectionId, CancellationToken cancellationToken = default);
}
