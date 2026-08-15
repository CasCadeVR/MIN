using MIN.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Messaging.Contracts.Enums;

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
    /// Список на исключение из broadcast
    /// </summary>
    public IEnumerable<Guid>? BroadcastExcludeIds { get; init; }

    /// <summary>
    /// Направление сообщения сессии
    /// </summary>
    public SessionMessageRoute Route { get; init; }

    /// <summary>
    /// Канал, по которому пойдёт сообщение
    /// </summary>
    public MessageChannel Channel { get; init; }

    /// <summary>
    /// Содержимое сообщения
    /// </summary>
    public required byte[] Body { get; init; }
}
