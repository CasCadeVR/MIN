namespace MIN.Sessions.Core.Messaging.Contracts.Models;

/// <summary>
/// Обёртка над между-процессорными сообщениями
/// </summary>
public class IpcMessageEnvelope
{
    /// <summary>
    /// Получатель сообщения внутри подкомнаты
    /// </summary>
    /// <remarks>
    /// null = broadcast
    /// </remarks>
    public Guid? RecipientId { get; init; }

    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public required byte[] Body { get; init; }

    /// <summary>
    /// Является ли сообщение широковещательным
    /// </summary>
    public bool IsBroadcast => RecipientId == null;
}
