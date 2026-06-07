using MIN.Sessions.Core.Transport.Contracts.Enums;

namespace MIN.Sessions.Core.Services.Contracts.Interfaces;

/// <summary>
/// Инициализатор приложений для сессии
/// </summary>
public interface ISessionProcessInitializer
{
    /// <summary>
    /// Запустить приложение сессии
    /// </summary>
    /// <returns>
    /// true - если успешно инициализировано и приложение готово слушать запросы
    /// false - если обратное
    /// </returns>
    Task<bool> StartAsync(Guid roomId, int subRoomId, string gameExePath, SessionProcessRole sessionProcessRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остоновить приложение сессии
    /// </summary>
    Task StopAsync(Guid roomId, int subRoomId, SessionProcessRole sessionProcessRole);
}
