using MIN.Common.Core.Contracts.Interfaces;
using MIN.Common.Core.Extensions;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Voice.Messaging;

/// <summary>
/// Сообщение старта звонка (по сути и есть звонок)
/// </summary>
public sealed class VoiceCallStartedMessage : BaseMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.VoiceCallStarted;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; set; }

    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public ParticipantInfo Sender { get; set; } = null!;

    /// <summary>
    /// Время завершения
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Закончился ли звонок
    /// </summary>
    public bool IsEnded => EndedAt != null;

    string IDescribable.GetDescription() => IsEnded
        ? $"{Sender.Name} начал звонок в комнате продолжительностью {(Timestamp - EndedAt!).Value.ToFriendlyString()}"
        : $"{Sender.Name} начал звонок в комнате";
}
