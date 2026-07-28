using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Discovery.Services.Contracts.Interfaces;

/// <summary>
/// Сервис обнаружения комнат в сети
/// </summary>
public interface IDiscoveryService
{
    /// <summary>
    /// Запустить процесс обнаружения своей комнаты в сети
    /// </summary>
    Task StartDiscoveryAsync(RoomInfo room, IEnumerable<IEndpoint> endpoints, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить процесс обнаружения своей комнаты в сети
    /// </summary>
    Task StopDiscoveryAsync(Guid roomId);

    /// <summary>
    /// Обнаружить комнаты
    /// </summary>
    Task DiscoverRoomsAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
}
