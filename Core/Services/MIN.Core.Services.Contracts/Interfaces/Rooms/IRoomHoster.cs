using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Events;

namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис для хостинга комнат (серверная сторона)
/// </summary>
public interface IRoomHoster
{
    /// <summary>
    /// Событие получения сырых данных от сервера
    /// </summary>
    event EventHandler<RoomRawMessageReceivedEventArgs>? RawMessageReceived;

    /// <summary>
    /// Событие изменения состояния соединения
    /// </summary>
    event EventHandler<RoomConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Начать хостинг комнаты
    /// </summary>
    Task StartHostingAsync(RoomInfo roomInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить хостинг комнаты
    /// </summary>
    Task StopHostingAsync(Guid roomId);

    /// <summary>
    /// Получить идентификатор соединения для комнаты
    /// </summary>
    Guid GetConnectionIdByRoomId(Guid roomId);

    /// <summary>
    /// Активен ли хостинг для указанной комнаты
    /// </summary>
    bool IsHosting(Guid roomId);
}
