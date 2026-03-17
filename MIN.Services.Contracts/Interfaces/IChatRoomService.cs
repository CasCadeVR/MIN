using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Events;
using MIN.Services.Contracts.Models.Messages;
using MIN.Services.Contracts.Models.Participants;
using MIN.Services.Contracts.Models.Rooms;

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
        IAsyncEnumerable<DiscoveredRoom> DiscoverAvailableRoomsAsync(IEnumerable<string> targetPCNames, int timeoutMs = 1000);

        /// <summary>
        /// Создать новую комнату и стать хостом
        /// </summary>
        Task<Room> CreateRoomAsync(string roomName, int maxParticipants, Participant host, CancellationToken cancellationToken = default);

        /// <summary>
        /// Подключиться к существующей комнате
        /// </summary>
        Task JoinRoomAsync(Room room, Participant participant, int timeoutMs = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить сообщение в комнату
        /// </summary>
        Task SendMessageAsync(string content, MessageType type = MessageType.Text, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить обновлённую статистику комнаты от сервера
        /// </summary>
        Task GetUpdatedRoomInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить обновлённые данные о комнате
        /// </summary>
        Task SendUpdateRoomRequestAsync(RoomInfoRequestMessage request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отключиться от комнаты
        /// </summary>
        Task DisconnectAsync(CancellationToken cancellationToken = default);
    }
}
