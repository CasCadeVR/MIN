using MIN.Core.Events.Contracts;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Получена информация о файле в комнате, после чего хост может раздать его другим
/// </summary>
public sealed class FilePendingMetaDataReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор потока, по которому придёт файл
    /// </summary>
    public Guid TransferId { get; set; }
}
