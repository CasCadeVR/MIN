using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Событие конца передачи файла
/// </summary>
public sealed record FileTransferCompletedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор сообщения метаданных
    /// </summary>
    public required Guid FileMetadataId { get; set; }

    /// <summary>
    /// Идентификатор инициатора файла
    /// </summary>
    public required Guid SenderId { get; set; }

    /// <summary>
    /// Путь к файлу
    /// </summary>
    /// <remarks>
    /// null если участник upload. Возьми из fileMetadata что ты отправил
    /// </remarks>
    public string? FilePath { get; set; }
}
