using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Interfaces.Messaging;

/// <summary>
/// Сервис по отправке сообщений
/// </summary>
public interface IMessageSender
{
    /// <summary>
    /// Отправляет сообщение через указанное соединение
    /// </summary>
    Task SendAsync(IMessage message, Guid roomId, Guid senderId, Guid recipientConnectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить сообщение всем участникам комнаты
    /// </summary>
    Task BroadcastAsync(IMessage message, Guid roomId, Guid senderId, IEnumerable<Guid>? excludeConnectionIds, CancellationToken cancellationToken = default);
}
