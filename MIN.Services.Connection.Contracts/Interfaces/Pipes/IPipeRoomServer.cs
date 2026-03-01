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
        /// Отправить сообщения определённому участнику
        /// </summary>
        Task SendMessageAsync<T>(ClientConnection sender, T message, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Отправить серверное сообщение всем участникам
        /// </summary>
        Task BroadcastMessageAsync<T>(ClientConnection sender, T message, CancellationToken cancellationToken = default) where T : class;

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
