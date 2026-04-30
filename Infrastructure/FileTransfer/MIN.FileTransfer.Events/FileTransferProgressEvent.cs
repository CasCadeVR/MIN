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
    /// Получено байтов
    /// </summary>
    public long BytesReceived { get; set; }

    /// <summary>
    /// Всего байтов
    /// </summary>
    public long TotalBytes { get; set; }
}
