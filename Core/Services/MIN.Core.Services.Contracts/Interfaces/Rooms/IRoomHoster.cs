using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Transport.Contracts.Models;

namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис для хостинга комнат (серверная сторона)
/// </summary>
public interface IRoomHoster : IRoomConnectionRelated
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
    Task<Room> StartHostingAsync(RoomInfo roomInfo, NetworkOptions networkOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить хостинг комнаты
    /// </summary>
    Task StopHostingAsync(Guid roomId);

    /// <summary>
    /// Активен ли хостинг для указанной комнаты
    /// </summary>
    bool IsHosting(Guid roomId);
}
