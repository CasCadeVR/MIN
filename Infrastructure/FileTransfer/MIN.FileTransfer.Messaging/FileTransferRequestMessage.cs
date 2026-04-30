using MIN.Core.Messaging.Contracts;
using MIN.FileTransfer.Services.Contracts.Models.Enums;

namespace MIN.FileTransfer.Messaging;

/// <summary>
/// Запрос на передачу файла
/// </summary>
public class FileTransferRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.FileTransferRequest;

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
    /// Направление передачи файла
    /// </summary>
    public FileTransferDirection Direction { get; set; }
}
