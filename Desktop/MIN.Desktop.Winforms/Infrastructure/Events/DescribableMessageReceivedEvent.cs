using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Desktop.Infrastructure.Events;

/// <summary>
/// Событие, возникающее при получении описаемого сообщения в комнате
/// </summary>
public sealed class DescribableMessageReceivedEvent : BaseEvent
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Описаемое сообщение
    /// </summary>
    public IDescribable DescribableMessage { get; init; } = null!;
}
