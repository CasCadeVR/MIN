using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Discovery.Services.Contracts.Enums;

namespace MIN.Discovery.Services.Contracts.Interfaces;

/// <summary>
/// Сервис обнаружения комнат
/// </summary>
public interface IDiscoveryService
{
    /// <summary>
    /// По какому методу будет поиск
    /// </summary>
    DiscoveryMethod DiscoveryMethod { get; }

    /// <summary>
    /// Запустить процесс обнаружения своей комнаты
    /// </summary>
    Task StartDiscoveryAsync(RoomInfo room, IEnumerable<IEndpoint> endpoints, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить процесс обнаружения своей комнаты
    /// </summary>
    Task StopDiscoveryAsync(Guid roomId);

    /// <summary>
    /// Обнаружить комнаты
    /// </summary>
    Task DiscoverRoomsAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
}
