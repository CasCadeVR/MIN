using MIN.Services.Contracts.Models;

namespace MIN.Services.Extensions
{
    /// <summary>
    /// Расширения для <see cref="ChatMessage"/>
    /// </summary>
    public static class ChatMessageExtensions
    {
        /// <summary>
        /// Создаёт полную копию сообщения для сериализации
        /// </summary>
        public static ChatMessage GetSerializableCopy(this ChatMessage message)
            => new()
            {
                Id = message.Id,
                SenderName = message.SenderName,
                SenderPCName = message.SenderPCName,
                Time = message.Time,
                TimestampUtc = message.TimestampUtc,
                MessageType = message.MessageType,
                Content = message.Content,
            };
    }
}
