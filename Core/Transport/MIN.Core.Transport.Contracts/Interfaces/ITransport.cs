using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Models;

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
    Task<Guid> StartHostingAsync(NetworkOptions networkOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все точки подключения
    /// </summary>
    Task<IEnumerable<IEndpoint>> GetEndpoints(Guid connectionId);

    /// <summary>
    /// Прекратить сервер для указанного соединения
    /// </summary>
    Task StopHostingAsync(Guid connectionId);

    /// <summary>
    /// Подключиться к удалённому устройству
    /// </summary>
    Task<Guid> ConnectAsync(IEndpoint endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Разорвать соединение с указанным соединением
    /// </summary>
    Task DisconnectClientAsync(Guid clientConnectionId, Guid? serverConnectionId);

    /// <summary>
    /// Отключиться
    /// </summary>
    Task DisconnectAsync(Guid connectionId);
}
