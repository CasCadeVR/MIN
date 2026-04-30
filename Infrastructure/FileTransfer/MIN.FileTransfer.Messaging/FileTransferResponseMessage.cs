using MIN.Core.Messaging.Contracts;

namespace MIN.FileTransfer.Messaging;

/// <summary>
/// Ответ на передачу файла
/// </summary>
public class FileTransferResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.FileTransferResponse;

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
    /// Можно ли начинать передачу файла
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}
