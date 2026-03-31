using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;

namespace MIN.Chat.Services
{
    /// <inheritdoc />
    public sealed class ChatService : IChatService
    {
        private readonly IMessageSender messageSender;
        private readonly IRoomRegistry roomRegistry;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ChatService"/>
        /// </summary>
        public ChatService(
            IMessageSender messageSender,
            IRoomRegistry roomRegistry)
        {
            this.messageSender = messageSender;
            this.roomRegistry = roomRegistry;
        }

        async Task IChatService.SendMessageAsync(Guid roomId, Guid connectionId, string content, ParticipantInfo sender)
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
            };

            await messageSender.SendAsync(message, roomId, connectionId, CancellationToken.None);
        }

        IReadOnlyList<ChatTextMessage> IChatService.GetMessageHistory(Guid roomId)
        {
            if (!roomRegistry.TryGetRoom(roomId, out var room))
            {
                return new List<ChatTextMessage>();
            }

            return room.ChatHistory.OfType<ChatTextMessage>().ToList().AsReadOnly();
        }

        IReadOnlyList<ParticipantInfo> IChatService.GetParticipants(Guid roomId)
        {
            if (!roomRegistry.TryGetRoom(roomId, out var room))
            {
                return new List<ParticipantInfo>();
            }

            return room.CurrentParticipants.Select(x => new ParticipantInfo(x)).ToList().AsReadOnly();
        }
    }
}
