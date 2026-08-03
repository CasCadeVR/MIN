using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;
using MIN.FileTransfer.Messaging;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Получена информация о файле в комнате
/// </summary>
public sealed record FileMetaDataMessageReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Полученная информация о файле
    /// </summary>
    public FileMetadataMessage Message { get; init; } = null!;
}
