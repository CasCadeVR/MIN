namespace MIN.Sessions.Core.Messaging.Contracts.Models;

/// <summary>
/// Обёртка над между-процессорными сообщениями, поставляемые приложением в MIN
/// </summary>
public class IpcProcessMessageEnvelope
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
}
