using MIN.Core.Events.Contracts;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Событие прогресса передачи файла
/// </summary>
public sealed class FileTransferProgressEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор потока, по которому придёт файл
    /// </summary>
    public Guid TransferId { get; set; }

    /// <summary>
    /// Идентификатор сообщения метаданных
    /// </summary>
    public Guid FileMetadataId { get; set; }

    /// <summary>
    /// Уже получено байтов
    /// </summary>
    public long BytesReceived { get; set; }
}
