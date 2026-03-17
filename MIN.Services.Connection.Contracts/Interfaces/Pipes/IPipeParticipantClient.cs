using MIN.Services.Contracts.Models.Messages;
using MIN.Services.Contracts.Models.Participants;
using MIN.Services.Contracts.Models.Rooms;

namespace MIN.Services.Connection.Contracts.Interfaces.Pipes
{
    /// <summary>
    /// Представление клиента в виде pipe
    /// </summary>
    public interface IPipeParticipantClient : IAsyncDisposable
    {
        /// <summary>
        /// Подключиться к серверу
        /// </summary>
        Task ConnectAsync(Room room, Participant selfParticipant, int timeoutMs = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отключиться от сервера
        /// </summary>
        Task DisconnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить сообщения
        /// </summary>
        Task SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить обновлённую статистику комнаты от сервера
        /// </summary>
        Task GetUpdatedRoomInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить обновлённые данные о комнате
        /// </summary>
        Task SendUpdatedRoomRequestAsync(RoomInfoRequestMessage request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Активен
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// В какой комнате сейчас находится участник
        /// </summary>
        Room? Room { get; }

        event EventHandler<ChatMessage>? MessageReceived;

        event EventHandler<Room>? RoomInfoReceived;

        event EventHandler<Participant>? ParticipantJoined;

        event EventHandler<Participant>? ParticipantLeft;

        event EventHandler? Disconnected;
    }
}
