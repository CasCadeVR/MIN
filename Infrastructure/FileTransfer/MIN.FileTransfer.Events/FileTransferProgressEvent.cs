using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Событие прогресса передачи файла
/// </summary>
public sealed record FileTransferProgressEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Уже получено байтов
    /// </summary>
    public long BytesReceived { get; set; }
}
