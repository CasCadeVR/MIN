using System.IO.Pipes;
using MIN.Services.Connection.Contracts.Models;
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
        Task SendMessageAsync(ClientConnection sender, ChatMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить серверное сообщение
        /// </summary>
        Task BroadcastMessageAsync(ClientConnection sender, ChatMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Активен
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Привязанная к серверу комната
        /// </summary>
        Room Room { get; }
    }
}
