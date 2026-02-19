using MIN.Services.Contracts.Models;

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
        /// <returns></returns>
        Task DisconnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить сообщения
        /// </summary>
        Task SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Активен
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// В какой комнате сейчас находиться участник
        /// </summary>
        Room? Room { get; }

        event EventHandler<ChatMessage>? MessageReceived;

        event EventHandler<RoomInfoMessage>? RoomInfoReceived;

        event EventHandler<Participant>? ParticipantJoined;

        event EventHandler<Participant>? ParticipantLeft;
        
        event EventHandler? Disconnected;
    }
}
