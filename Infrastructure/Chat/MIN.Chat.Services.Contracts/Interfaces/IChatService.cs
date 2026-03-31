using MIN.Chat.Messaging;
using MIN.Core.Entities.Contracts.Models;

namespace MIN.Chat.Services.Contracts.Interfaces
{
    /// <summary>
    /// Сервис для работы с чатом
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Отправить текстовое сообщение
        /// </summary>
        Task SendMessageAsync(Guid roomId, Guid connectionId, string content, ParticipantInfo sender);

        /// <summary>
        /// Получить историю сообщений из кэша
        /// </summary>
        IReadOnlyList<ChatTextMessage> GetMessageHistory(Guid roomId);

        /// <summary>
        /// Получить список участников комнаты из кэша
        /// </summary>
        IReadOnlyList<ParticipantInfo> GetParticipants(Guid roomId);
    }
}
