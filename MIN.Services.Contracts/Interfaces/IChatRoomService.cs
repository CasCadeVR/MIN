using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Events;

namespace MIN.Services.Contracts.Interfaces
{
    /// <summary>
    /// Сервис по работе с подключениями
    /// </summary>
    public interface IChatRoomService
    {
        event EventHandler<ParticipantJoinedEventArgs>? ParticipantJoined;
        event EventHandler<ParticipantLeftEventArgs>? ParticipantLeft;
        event EventHandler<MessageReceivedEventArgs>? MessageReceived;
        event EventHandler<RoomStateChangedEventArgs>? RoomStateChanged;
        event EventHandler<ConnectionLostEventArgs>? ConnectionLost;

        /// <summary>
        /// Получить список найденных комнат
        /// </summary>
        Task<IEnumerable<Room>> DiscoverAvailableRoomsAsync(IEnumerable<string> targetPCNames, int timeoutMs = 1000);

        /// <summary>
        /// Создать новую комнату и стать хостом
        /// </summary>
        Task CreateRoomAsync(string roomName, int maxParticipants, Participant host);

        /// <summary>
        /// Подключиться к существующей комнате
        /// </summary>
        Task JoinRoomAsync(Guid roomId, Participant participant);

        /// <summary>
        /// Отправить сообщение в текущую комнату
        /// </summary>
        Task SendMessageAsync(string content, MessageType type = MessageType.Text);

        /// <summary>
        /// Отключиться от комнаты
        /// </summary>
        Task DisconnectAsync();
    }
}
