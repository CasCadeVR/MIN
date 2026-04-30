using MIN.Core.Messaging.Contracts;

namespace MIN.FileTransfer.Messaging;

/// <summary>
/// Сообщение отмены передачи файла
/// </summary>
public class FileTransferCancelMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.FileTransferCancel;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Идентификатор комнаты, в которую отправлено сообщение
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Идентификатор потока, по которому придёт файл
    /// </summary>
    public Guid TransferId { get; set; }

    /// <summary>
    /// Причина отмены
    /// </summary>
    public string? Reason { get; set; }
}
