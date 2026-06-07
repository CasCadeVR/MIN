using MIN.Sessions.Core.Transport.Contracts.Events;
using MIN.Sessions.Core.Transport.Contracts.Models;

namespace MIN.Sessions.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Транспорт для междупроцессорного общения
/// </summary>
public interface ISessionProcessTransport : IAsyncDisposable
{
    /// <summary>
    /// Запустить серверную часть транспорта
    /// </summary>
    Task StartAsync(Guid roomId, CancellationToken ct = default);

    /// <summary>
    /// Событие: получены сырые данные от подключения
    /// </summary>
    event EventHandler<ProcessTransportMessageEventArgs>? MessageReceived;

    /// <summary>
    /// Параметр подключения для игрового процесса
    /// </summary>
    /// <remarks>
    /// Например: {"pipe":"MIN_{roomId}_{sessionType}"} {"tcp":"127.0.0.1:54321"}
    /// </remarks>
    string GetConnectionString();

    /// <summary>
    /// Ждать подключения одного процесса (server или client)
    /// </summary>
    Task<TransportConnection> WaitForConnectionAsync(
        Guid roomId, int subRoomId, string role, CancellationToken ct = default);

    /// <summary>
    /// Отправить данные приложению
    /// </summary>
    Task SendAsync(Guid roomId, int subRoomId, string role, byte[] data, CancellationToken ct);

    /// <summary>
    /// Закрыть конкретное подключение
    /// </summary>
    Task DisconnectAsync(Guid roomId, int subRoomId, string role);

    /// <summary>
    /// Остановить транспорт, разорвать все соединения
    /// </summary>
    Task StopAsync();
}
