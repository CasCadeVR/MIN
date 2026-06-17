using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
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
    /// Идентификатор потока, по которому мы ожидаем, что придёт файл
    /// </summary>
    public Guid TransferId { get; set; }

    /// <summary>
    /// Идентификатор сообщения метаданных, на которое ссылается запрос
    /// </summary>
    public Guid FileMetadataId { get; set; }

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Направление передачи файла
    /// </summary>
    public FileTransferDirection Direction { get; set; }
}
