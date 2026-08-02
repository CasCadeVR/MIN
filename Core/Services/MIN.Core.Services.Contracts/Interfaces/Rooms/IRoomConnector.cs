using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис для подключения к удалённым комнатам (клиентская сторона)
/// </summary>
public interface IRoomConnector : IRoomConnectionRelated
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
    /// Подключиться к удалённой комнате
    /// </summary>
    Task<ConnectionResult> ConnectAsync(IEndpoint endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отключиться от удалённой комнаты
    /// </summary>
    Task DisconnectAsync(Guid roomId, Guid connectionId, DisconnectReason reason);

    /// <summary>
    /// Подключен ли к указанной комнате
    /// </summary>
    bool IsConnected(Guid roomId);
}
