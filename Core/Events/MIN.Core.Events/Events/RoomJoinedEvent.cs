using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;

namespace MIN.Core.Events.Events;

/// <summary>
/// Событие, возникающее при заходе в комнату как клиент, получив всю необходимую для показа информацию
/// </summary>
public sealed class RoomJoinedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Актуальная информация о комнате
    /// </summary>
    /// <remarks>
    /// Просто если мы увидели, что мест 3, а хост сменил на 2, то надо обновить
    /// </remarks>
    public RoomInfo RoomInfo { get; init; } = null!;
}
