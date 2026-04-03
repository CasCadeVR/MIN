using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Stores;

namespace MIN.Chat.Services
{
    /// <inheritdoc />
    public sealed class ChatService : IChatService
    {
        private readonly IMessageSender messageSender;
        private readonly IParticipantStore participantStore;
        private readonly IMessageStore messageStore;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ChatService"/>
        /// </summary>
        public ChatService(IMessageSender messageSender,
            IParticipantStore participantStore,
            IMessageStore messageStore)
        {
            this.messageSender = messageSender;
            this.participantStore = participantStore;
            this.messageStore = messageStore;
        }

        async Task IChatService.SendMessageAsync(Guid roomId, Guid connectionId, string content, ParticipantInfo sender, Guid? recipientId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Message content cannot be empty", nameof(content));
            }

            var message = new ChatTextMessage
            {
                RoomId = roomId,
                Sender = sender,
                Content = content,
                RecipientId = recipientId,
            };

            await messageSender.SendAsync(message, roomId, sender.Id, connectionId, CancellationToken.None);
        }

        IReadOnlyList<ChatTextMessage> IChatService.GetChatTextMessageHistory(Guid roomId, int? page, int? pageSize)
            => messageStore.GetHistory(roomId, page, pageSize).OfType<ChatTextMessage>().ToList().AsReadOnly();

        IReadOnlyList<ParticipantInfo> IChatService.GetParticipants(Guid roomId)
            => participantStore.GetParticipants(roomId).ToList().AsReadOnly();
    }
}
