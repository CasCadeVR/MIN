using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Contracts.Events;

namespace MIN.Messaging.Contracts.Interfaces;

/// <summary>
/// Сервис для отправки и получения сообщений, объединяющий транспорт, сериализацию и шифрование
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Событие, возникающее при получении сообщения (уже расшифрованного и десериализованного)
    /// </summary>
    event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Ассоциирует участника с идентификатором соединения
    /// </summary>
    void SetParticipantInfo(Guid connectionId, ParticipantInfo participant);

    /// <summary>
    /// Отправляет сообщение через указанное соединение
    /// </summary>
    Task SendAsync(IMessage message, Guid roomId, Guid connectionId, CancellationToken cancellationToken = default);
}
