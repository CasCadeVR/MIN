using MIN.Core.Events.Contracts;
using MIN.FileTransfer.Messaging;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Получена информация о файле в комнате
/// </summary>
public sealed class FileMetaDataMessageReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Полученная информация о файле
    /// </summary>
    public FileMetadataMessage Message { get; init; } = null!;
}
