using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Messaging;

/// <summary>
/// Сообщение готовности хостинга сессии
/// </summary>
public sealed class SessionReadyMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.SessionReady;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    /// <summary>
    /// Выбранная сессия
    /// </summary>
    public Session Session { get; set; } = null!;
}
