using MIN.Core.Transport.Contracts.Events;

namespace MIN.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Интерфейс транспортного уровня для передачи данных между участниками
/// </summary>
public interface ITransport
{
    /// <summary>
    /// Событие получения сырых данных от транспорта
    /// </summary>
    event EventHandler<RawMessageReceivedEventArgs>? RawMessageReceived;

    /// <summary>
    /// Событие изменения состояния соединения
    /// </summary>
    event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Отправить сырые данные конкретному участнику
    /// </summary>
    Task SendAsync(byte[] data, Guid roomId, Guid connectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить сырые данные всем участникам комнаты
    /// </summary>
    Task BroadcastAsync(byte[] data, Guid roomId, IEnumerable<Guid>? excludeConnections = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запустить сервер подключений для указанной комнаты
    /// </summary>
    Task StartHostingAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить точку подключения комнаты
    /// </summary>
    IEndpoint GetEndpoint(Guid roomId);

    /// <summary>
    /// Прекратить сервер для указанной комнаты
    /// </summary>
    Task StopHostingAsync(Guid roomId);

    /// <summary>
    /// Подключиться к удалённой комнате
    /// </summary>
    Task<Guid> ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отключиться от указанной комнаты
    /// </summary>
    Task DisconnectAsync(Guid roomId, Guid connectionId);
}
