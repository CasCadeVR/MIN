using MIN.Core.Entities.Contracts.Interfaces;

namespace MIN.Core.Entities.Contracts.Models;

/// <summary>
/// Данные комнаты для передачи по сети
/// </summary>
public record RoomInfo : IRoomData
{
    /// <inheritdoc />
    public Guid Id { get; init; }

    /// <inheritdoc />
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public int ParticipantCount { get; set; }

    /// <inheritdoc />
    public int MaximumParticipants { get; set; }

    /// <inheritdoc />
    public bool IsActive { get; set; }

    /// <inheritdoc />
    public ParticipantInfo HostParticipant { get; set; }

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

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomInfo"/>
    /// </summary>
    public RoomInfo()
    {
        Id = Guid.NewGuid();
    }
}
