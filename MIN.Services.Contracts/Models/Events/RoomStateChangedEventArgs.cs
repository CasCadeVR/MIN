using MIN.Services.Contracts.Models.Enums;
using MIN.Services.Contracts.Models.Rooms;

namespace MIN.Services.Contracts.Models.Events;

/// <summary>
/// Аргументы события изменения состояния комнаты
/// </summary>
public class RoomStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Комната
    /// </summary>
    public Room? Room { get; }

    /// <summary>
    /// Состояние комнаты
    /// </summary>
    public RoomState State { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomStateChangedEventArgs"/>
    /// </summary>
    public RoomStateChangedEventArgs(Room? Room, RoomState State)
    {
        this.Room = Room;
        this.State = State;
    }
}
