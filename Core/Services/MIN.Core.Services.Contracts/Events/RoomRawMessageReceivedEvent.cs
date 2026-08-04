using MIN.Core.Events.Contracts.Models;

namespace MIN.Core.Services.Contracts.Events;

/// <summary>
/// Аргументы события получения сырого сообщения внутри программы
/// </summary>
public sealed record RoomRawMessageReceivedEvent : BaseEvent
{
    /// <summary>
    /// Аргументы события
    /// </summary>
    public required RoomRawMessageReceivedEventArgs EventArgs { get; init; }
}
