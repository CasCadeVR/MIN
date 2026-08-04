using MIN.Core.Entities.Contracts.Enums;

namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис по измерению пинга и поддерживания состояния сети
/// </summary>
public interface IPingService
{
    /// <summary>
    /// Соединение разорвалось по причине connection timeout
    /// </summary>
    /// <remarks>
    /// Возвращает идентификатор комнаты и идентификатор соединения соответсвенно
    /// </remarks>
    event Func<Guid, Guid, Task>? OnConnectionTimeout;

    /// <summary>
    /// Зарегистрировать сессию отслеживания пинга
    /// </summary>
    Task RegisterHeartbeatSession(Role role, Guid roomId, Guid connectionId);

    /// <summary>
    /// Удалить сессию отслеживания пинга
    /// </summary>
    Task UnregisterHeartbeatSession(Role role, Guid roomId, Guid connectionId);
}
