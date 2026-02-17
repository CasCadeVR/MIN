using System.IO.Pipes;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Contracts.Interfaces.Pipes
{
    /// <summary>
    /// Представление сервера в виде pipe
    /// </summary>
    public interface IPipeRoomServer
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
        Task SendMessageAsync(NamedPipeServerStream pipe, ChatMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить серверное сообщение
        /// </summary>
        Task BroadcastSystemMessageAsync(NamedPipeServerStream pipe, string content, CancellationToken cancellationToken = default);

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
