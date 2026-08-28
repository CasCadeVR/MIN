using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Событие ошибки передачи файла
/// </summary>
public sealed record FileTransferFailedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор сообщения метаданных
    /// </summary>
    public Guid FileMetadataId { get; set; }

    /// <summary>
    /// Идентификатор отправителя запроса на скачивание
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}
