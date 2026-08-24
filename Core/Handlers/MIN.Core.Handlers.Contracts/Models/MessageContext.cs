using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Stores.Contracts.Models;

namespace MIN.Core.Handlers.Contracts.Models;

/// <summary>
/// Контекст обработки сообщения
/// </summary>
public sealed class MessageContext
{
    /// <summary>
    /// Контекст комнаты, в которой было получено сообщение
    /// </summary>
    public RoomContext RoomContext { get; init; }

    /// <summary>
    /// Идентификатор участника локального пользователя
    /// </summary>
    public Guid SelfId { get; init; }

    /// <summary>
    /// Идентификатор соеднинения, по которому пришло сообщение
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Роль получения сообщения
    /// </summary>
    public Role Role { get; init; }

    /// <summary>
    /// Токен отмены для длительных операций
    /// </summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Инициализирует новый экзмепляр <see cref="MessageContext"/>
    /// </summary>
    public MessageContext(RoomContext roomContext, Guid selfId, Guid connectionId, Role role, CancellationToken cancellationToken)
    {
        RoomContext = roomContext;
        SelfId = selfId;
        ConnectionId = connectionId;
        Role = role;
        CancellationToken = cancellationToken;
    }
}
