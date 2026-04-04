using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис для хостинга комнат (серверная сторона)
/// </summary>
public interface IRoomHoster
{
    /// <summary>
    /// Начать хостинг комнаты
    /// </summary>
    Task StartHostingAsync(RoomInfo roomInfo, IEndpoint endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить хостинг комнаты
    /// </summary>
    Task StopHostingAsync(Guid roomId);

    /// <summary>
    /// Активен ли хостинг для указанной комнаты
    /// </summary>
    bool IsHosting(Guid roomId);
}
