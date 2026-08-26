using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Core.Messaging.Stateless.RoomRelated.History;

/// <summary>
/// Действие удаления истории чата
/// </summary>
public sealed class ChatHistoryClearMessage : BaseMessage, IDescribable
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.ChatHistoryClear;

    /// <inheritdoc />
    public override bool RequiresLocalDuplication => true;

    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// Время первого сообщения
    /// </summary>
    public DateTime From { get; set; }

    /// <summary>
    /// Время, по которое все сообщения будут удалены
    /// </summary>
    public DateTime UpTo { get; set; }

    string IDescribable.GetDescription() => $"История была очищена c {From:f} по {UpTo:f}";
}
