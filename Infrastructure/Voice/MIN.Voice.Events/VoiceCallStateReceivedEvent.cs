using MIN.Core.Entities;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Models;

namespace MIN.Voice.Events;

/// <summary>
/// Получена информация о звонкк в комнате
/// </summary>
public sealed record VoiceCallStateReceivedEvent : BaseEvent, IRoomScopedEvent
{
    /// <inheritdoc />
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    /// <remarks>
    /// null, если активного звонка неут, иначе - id подкомнаты звонка
    /// </remarks>
    public int? ActiveSubRoomId { get; set; }

    /// <summary>
    /// Когда начался звонок
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Участники в звонке
    /// </summary>
    public List<Participant> CallParticipants { get; set; } = [];
}
