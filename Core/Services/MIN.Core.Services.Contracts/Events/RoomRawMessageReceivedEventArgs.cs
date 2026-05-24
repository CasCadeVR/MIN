using MIN.Core.Transport.Contracts.Events;

namespace MIN.Core.Services.Contracts.Events;

/// <summary>
/// Аргументы события получения сырых данных от транспорта
/// </summary>
public sealed class RoomRawMessageReceivedEventArgs : RawMessageReceivedEventArgs
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomRawMessageReceivedEventArgs"/>
    /// </summary>
    public RoomRawMessageReceivedEventArgs(Guid roomId, RawMessageReceivedEventArgs args)
        : base(args.Data, args.ConnectionId, args.ServerConnectionId)
    {
        RoomId = roomId;
    }
}
