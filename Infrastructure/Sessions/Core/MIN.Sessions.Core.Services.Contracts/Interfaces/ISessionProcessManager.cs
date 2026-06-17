using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Services.Contracts.Interfaces;

/// <summary>
/// Инициализатор приложений для сессии
/// </summary>
public interface ISessionProcessManager
{
    /// <summary>
    /// Запустить приложение сессии
    /// </summary>
    /// <returns>
    /// true - если успешно инициализировано и приложение готово слушать запросы
    /// false - если обратное
    /// </returns>
    Task<bool> StartAsync(string gameExePath, ProcessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить приложение сессии
    /// </summary>
    Task StopAsync(ProcessContext context);

    /// <summary>
    /// Остановить все приложения сессии, связанные с комнатой
    /// </summary>
    Task StopForRoomAsync(Guid roomId);

    /// <summary>
    /// Остановить все приложения сессии
    /// </summary>
    Task StopAllAsync();
}
