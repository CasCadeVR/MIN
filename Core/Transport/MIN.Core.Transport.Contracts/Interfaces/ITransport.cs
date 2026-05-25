using MIN.Core.Transport.Contracts.Events;

namespace MIN.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Интерфейс транспортного уровня для передачи данных между устройствами
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
    /// Отправить сырые данные соединению
    /// </summary>
    /// <remarks>
    /// serverConnectionId указывает, какой id соединения у сервера в случае хоста
    /// </remarks>
    Task SendAsync(byte[] data, Guid receipientConnectionId, Guid? serverConnectionId, CancellationToken cancellationToken);

    /// <summary>
    /// Отправить сырые данные всем соединениям
    /// </summary>
    Task BroadcastAsync(byte[] data, Guid connectionId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken);

    /// <summary>
    /// Запустить сервер подключений
    /// </summary>
    Task<Guid> StartHostingAsync(bool withPortForwarding, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить точку подключения
    /// </summary>
    IEndpoint GetEndpoint(Guid connectionId);

    /// <summary>
    /// Прекратить сервер для указанного соединения
    /// </summary>
    Task StopHostingAsync(Guid connectionId);

    /// <summary>
    /// Подключиться к удалённому устройству
    /// </summary>
    Task<Guid> ConnectAsync(IEndpoint endpoint, int timeoutMs = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Разорвать соединение с указанным соединением
    /// </summary>
    Task DisconnectClientAsync(Guid clientConnectionId, Guid? serverConnectionId, string reason);

    /// <summary>
    /// Отключиться
    /// </summary>
    Task DisconnectAsync(Guid connectionId);
}
