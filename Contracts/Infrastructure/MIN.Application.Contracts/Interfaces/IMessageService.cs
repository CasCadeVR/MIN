using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Contracts.Events;
using MIN.Messaging.Contracts.Interfaces;

namespace MIN.Application.Contracts.Interfaces;

/// <summary>
/// Сервис для отправки и получения сообщений, объединяющий транспорт, сериализацию и шифрование
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Отправляет сообщение через указанное соединение
    /// </summary>
    Task SendAsync(IMessage message, Guid roomId, Guid connectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить сырые данные всем участникам комнаты
    /// </summary>
    Task BroadcastAsync(IMessage message, Guid roomId, IEnumerable<Guid>? excludeConnections = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ассоциирует участника с идентификатором соединения
    /// </summary>
    void SetParticipantInfo(Guid connectionId, ParticipantInfo participant);
}
