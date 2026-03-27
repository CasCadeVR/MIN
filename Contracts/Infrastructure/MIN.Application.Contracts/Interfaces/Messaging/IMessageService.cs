using MIN.Entities.Contracts.Entities;

namespace MIN.Services.Contracts.Interfaces.Messaging;

/// <summary>
/// Сервис для отправки и получения сообщений, объединяющий транспорт, сериализацию и шифрование
/// </summary>
public interface IMessageService : IMessageSender, IMessageReceiver
{
    /// <summary>
    /// Ассоциирует участника с идентификатором соединения
    /// </summary>
    void SetParticipantInfo(Guid connectionId, ParticipantInfo participant);
}
