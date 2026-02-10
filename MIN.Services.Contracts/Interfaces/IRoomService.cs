using MIN.Services.Contracts.Models;

namespace MIN.Services.Contracts.Interfaces;

/// <summary>
/// Сервис по работе с комнатами
/// </summary>
public interface IRoomService
{
    /// <summary>
    /// Получить список <see cref="Room"/>
    /// </summary>
    public Task<IEnumerable<Room>> GetAll(CancellationToken cancellationToken);

    /// <summary>
    /// Создать новую <see cref="Room"/>
    /// </summary>
    public Task<Room> Create(Room room, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить <see cref="Room"/>
    /// </summary>
    public Task<Room> Update(Room room, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить <see cref="Room"/>
    /// </summary>
    public Task Delete(Room room, CancellationToken cancellationToken);
}
