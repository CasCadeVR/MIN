using MIN.Core.Events.Contracts;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Событие ошибки передачи файла
/// </summary>
public sealed class FileTransferFailedEvent : BaseEvent
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
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}
