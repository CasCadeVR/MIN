using MIN.Core.Events.Contracts;

namespace MIN.Sessions.Chess.Events;

/// <summary>
/// События получения ответа от присоединения к шахматам
/// </summary>
public sealed class ChessJoinResponseReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; init; }

    /// <summary>
    /// Текущая ситуация на доске
    /// </summary>
    public string CurrentPositionOnBoard { get; set; } = string.Empty;
}
