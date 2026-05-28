using MIN.Core.Entities.Contracts.Models;
using MIN.Core.SubRooms.Contracts.Enums;

namespace MIN.Core.SubRooms.Contracts.Models;

/// <summary>
/// Инфорация о подкомнате
/// </summary>
public record SubRoomInfo
{
    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Цель подкомнаты
    /// </summary>
    public SubRoomPurpose Purpose { get; init; }

    /// <summary>
    /// Идентификатора запросидшего создания подкомнаты
    /// </summary>
    public Guid CreatorId { get; init; }

    /// <summary>
    /// Участники подкомнаты
    /// </summary>
    public List<ParticipantInfo> Participants { get; init; } = [];

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}
