using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models;

namespace MIN.Core.Services.Contracts.Interfaces.Lifecycle;

/// <summary>
/// Единый оркестратор жизненного цикла комнат: подключение/отключение (клиент) и хостинг (сервер)
/// </summary>
public interface IRoomLifecycleManager
{
    /// <summary>
    /// Подключиться к удалённой комнате (клиентская сторона)
    /// </summary>
    Task<ConnectionResult> ConnectAsync(IEndpoint endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отключиться от удалённой комнаты (клиентская сторона)
    /// </summary>
    Task DisconnectAsync(Guid roomId, Guid connectionId, DisconnectReason reason);

    /// <summary>
    /// Начать хостинг комнаты (серверная сторона)
    /// </summary>
    Task<Room> StartHostingAsync(RoomInfo roomInfo, NetworkOptions networkOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить настройки сети комнаты
    /// </summary>
    Task<IEnumerable<IEndpoint>> UpdateNetworkOptions(Guid roomId, NetworkOptions newNetworkOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить хостинг комнаты
    /// </summary>
    Task StopHostingAsync(Guid roomId);

    /// <summary>
    /// Кикнуть участника из комнаты
    /// </summary>
    Task KickClientAsync(Guid roomId, Guid participantId, DisconnectReason reason);
}
