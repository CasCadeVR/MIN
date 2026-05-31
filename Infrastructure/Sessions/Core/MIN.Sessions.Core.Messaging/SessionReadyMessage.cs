using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Messaging;

/// <summary>
/// Сообщение готовности хостинга сессии
/// </summary>
public sealed class SessionReadyMessage : BaseMessage, IDescribable
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
    /// Текущее количество участников в ней
    /// </summary>
    public int CurrentParticipantAmount { get; set; }

    /// <summary>
    /// Выбранная сессия
    /// </summary>
    public Session Session { get; set; } = null!;

    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public ParticipantInfo Sender { get; set; } = null!;

    string IDescribable.GetDescription() => $"{Sender.Name} запустил активность \"{Session.Name}\"";
}
