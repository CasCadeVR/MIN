namespace MIN.Sessions.Core.Messaging.Contracts.Models;

/// <summary>
/// Обёртка над между-процессорными сообщениями, поставляемые MIN в приложение
/// </summary>
public class IpcMinMessageEnvelope
{
    /// <summary>
    /// Отправитель сообщения внутри подкомнаты
    /// </summary>
    public Guid SenderId { get; init; }

    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public required byte[] Body { get; init; }
}
