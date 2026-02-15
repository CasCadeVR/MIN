using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Contracts.Interfaces.Pipes
{
    /// <summary>
    /// Представление сервера в виде pipe
    /// </summary>
    public interface IPipeRoomServer : IAsyncDisposable
    {
        /// <summary>
        /// Запустить сервер
        /// </summary>
        Task StartAsync(Room room, CancellationToken cancellationToken = default);

        /// <summary>
        /// Остановить сервер
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Отправить сообщения
        /// </summary>
        Task SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить серверное сообщение
        /// </summary>
        Task BroadcastSystemMessageAsync(string content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Активен
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Привязанная к серверу комната
        /// </summary>
        Room Room { get; }

        event EventHandler<ChatMessage> MessageReceived;
        event EventHandler<Participant> ParticipantJoined;
        event EventHandler<Participant> ParticipantLeft;
        event EventHandler<Participant> ClientDisconnected;
    }
}
