namespace MIN.Discovery.Services.Contracts.Interfaces;

/// <summary>
/// Сервис обнаружения комнат в сети
/// </summary>
public interface IDiscoveryService
{
    /// <summary>
    /// Запустить процесс обнаружения своей комнаты в сети
    /// </summary>
    Task StartDiscoveryAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить процесс обнаружения своей комнаты в сети
    /// </summary>
    Task StopDiscoveryAsync(Guid roomId);

    /// <summary>
    /// Обнаружить комнаты
    /// </summary>
    Task DiscoverRoomsAsync(IEnumerable<string>? computers, TimeSpan timeout, CancellationToken cancellationToken = default);
}
