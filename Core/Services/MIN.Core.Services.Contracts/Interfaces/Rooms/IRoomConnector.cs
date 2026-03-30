using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис для подключения к удалённым комнатам (клиентская сторона)
/// </summary>
public interface IRoomConnector
{
    /// <summary>
    /// Подключиться к удалённой комнате
    /// </summary>
    Task<Guid> ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отключиться от удалённой комнаты
    /// </summary>
    Task DisconnectAsync(Guid roomId, Guid connectionId);

    /// <summary>
    /// Подключен ли к указанной комнате
    /// </summary>
    bool IsConnected(Guid roomId);
}
