using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Chat.Services
{
    /// <inheritdoc />
    public sealed class ChatService : IChatService
    {
        private readonly IMessageRouter messageRouter;
        private readonly IParticipantStore participantStore;
        private readonly IMessageStore messageStore;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ChatService"/>
        /// </summary>
        public ChatService(IMessageRouter messageRouter,
            IParticipantStore participantStore,
            IMessageStore messageStore)
        {
            this.messageRouter = messageRouter;
            this.participantStore = participantStore;
            this.messageStore = messageStore;
        }

        async Task IChatService.SendMessageAsync(Guid roomId, string content, ParticipantInfo sender, Guid? recipientId, CancellationToken cancellationToken)
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

            await messageRouter.RouteAsync(message, roomId, sender.Id, Recipient.FromParticipant(roomId, recipientId), cancellationToken);
        }

        IReadOnlyList<ChatTextMessage> IChatService.GetChatTextMessageHistory(Guid roomId, int? page, int? pageSize)
            => messageStore.GetHistory(roomId, page, pageSize).OfType<ChatTextMessage>().ToList().AsReadOnly();

        IReadOnlyList<ParticipantInfo> IChatService.GetParticipants(Guid roomId)
            => participantStore.GetParticipants(roomId).ToList().AsReadOnly();
    }
}
