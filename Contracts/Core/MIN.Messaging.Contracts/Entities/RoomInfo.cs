using MIN.Entities.Contracts;

namespace MIN.Messaging.Contracts.Entities;

/// <summary>
/// Данные комнаты для передачи по сети
/// </summary>
public record RoomInfo : IRoomData
{
    /// <inheritdoc />
    public Guid Id { get; init; }

    /// <inheritdoc />
    public string Name { get; init; } = string.Empty;

    /// <inheritdoc />
    public int ParticipantCount { get; init; }

    /// <inheritdoc />
    public int MaximumParticipants { get; init; }

    /// <inheritdoc />
    public bool IsActive { get; init; }

    /// <inheritdoc />
    public IParticipantData HostParticipant { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomInfo"/>
    /// </summary>
    public RoomInfo(IRoomData room)
    {
        Id = room.Id;
        Name = room.Name;
        HostParticipant = room.HostParticipant;
        ParticipantCount = room.ParticipantCount;
        MaximumParticipants = room.MaximumParticipants;
        IsActive = room.IsActive;
    }
}
