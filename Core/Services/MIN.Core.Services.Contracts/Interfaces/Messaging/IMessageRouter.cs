using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Interfaces.Messaging;

/// <summary>
/// Маршрутизатор сообщений
/// </summary>
public interface IMessageRouter
{
    /// <summary>
    /// Направить сообщение в зависимости от контекста
    /// </summary>
    Task RouteAsync(IMessage message, Guid roomId, Guid senderId, Guid? recipientId, CancellationToken cancellationToken = default);
}
