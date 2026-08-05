using MIN.Core.Transport.Contracts.Events;

namespace MIN.Core.Services.Contracts.Events;

/// <summary>
/// Аргументы события изменения состояния соединения
/// </summary>
public sealed class RoomConnectionStateChangedEventArgs : ConnectionStateChangedEventArgs
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomConnectionStateChangedEventArgs"/>
    /// </summary>
    public RoomConnectionStateChangedEventArgs(Guid roomId, ConnectionStateChangedEventArgs args)
        : base(args.ConnectionId, args.IsConnected, args.DisconnectReason)
    {
        RoomId = roomId;
    }
}
