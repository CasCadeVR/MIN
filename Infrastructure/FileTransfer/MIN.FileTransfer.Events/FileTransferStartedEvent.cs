using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Событие начала передачи файла
/// </summary>
public sealed record FileTransferStartedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор сообщения метаданных
    /// </summary>
    public Guid FileMetadataId { get; set; }

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Инициатор отправления сообщения
    /// </summary>
    public ParticipantInfo Sender { get; set; } = null!;

    /// <summary>
    /// Направление передачи файла
    /// </summary>
    public FileTransferDirection Direction { get; set; }
}
