using MIN.Core.Messaging.Contracts;

namespace MIN.FileTransfer.Messaging;

/// <summary>
/// Сообщение конца передачи файла
/// </summary>
public class FileTransferCompleteMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.FileTransferComplete;

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
}
