namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Монитор состояния соединения
/// </summary>
public interface IConnectionMonitor
{
    /// <summary>
    /// Начать отслеживание состояния соединения
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить отслеживание состояния соединения
    /// </summary>
    Task StopAsync();
}
