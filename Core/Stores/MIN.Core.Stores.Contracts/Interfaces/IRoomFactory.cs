using MIN.Core.Stores.Contracts.Models;

namespace MIN.Core.Stores.Contracts.Interfaces;

/// <summary>
/// Фабрика комнат
/// </summary>
public interface IRoomFactory
{
    /// <summary>
    /// Создать контекст комнаты
    /// </summary>
    RoomContext GetOrCreateContext(Guid roomId);

    /// <summary>
    /// Уничтожить контекст комнаты
    /// </summary>
    void DestroyContext(Guid roomId);

    /// <summary>
    /// Получить контекст комнаты
    /// </summary>
    RoomContext? GetRoomContext(Guid roomId);

    /// <summary>
    /// Попытаться получить контекст комнаты
    /// </summary>
    bool TryGetContext(Guid roomId, out RoomContext? context);

    /// <summary>
    /// Получить все контексты комнат
    /// </summary>
    IEnumerable<RoomContext> GetAllContexts();
}
