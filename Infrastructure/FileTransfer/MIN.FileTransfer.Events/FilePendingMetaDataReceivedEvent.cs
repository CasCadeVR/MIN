using MIN.Core.Events.Contracts.Models;

namespace MIN.FileTransfer.Events;

/// <summary>
/// Получена информация о файле в комнате, после чего хост может раздать его другим
/// </summary>
public sealed record FilePendingMetaDataReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор потока, по которому придёт файл
    /// </summary>
    public Guid TransferId { get; set; }

    /// <summary>
    /// Путь к скаченному файлу
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
}
