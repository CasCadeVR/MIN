using MIN.FileTransfer.Services.Contracts.Models.Enums;

namespace MIN.FileTransfer.Services.Contracts.Models;

/// <summary>
/// Информация о передаче файла
/// </summary>
public sealed class TransferInfo
{
    /// <summary>
    /// Идентификатор комнаты, в которую отправлено сообщение
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор потока, по которому придёт файл
    /// </summary>
    public Guid TransferId { get; init; }

    /// <summary>
    /// Направление передачи файла
    /// </summary>
    public FileTransferDirection Direction { get; init; }

    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; init; } = string.Empty;
}
