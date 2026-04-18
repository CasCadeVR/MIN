using MIN.Core.Entities.Contracts.Interfaces;

namespace MIN.Core.Entities.Contracts.Models;

/// <summary>
/// Данные комнаты для передачи по сети
/// </summary>
public record RoomInfo : IRoomData
{
    /// <inheritdoc />
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public int ParticipantCount { get; set; }

    /// <inheritdoc />
    public int MaximumParticipants { get; set; }

    /// <inheritdoc />
    public bool IsActive { get; set; }

    /// <inheritdoc />
    public ParticipantInfo HostParticipant { get; set; } = null!;

    /// <inheritdoc />
    public DateTime CreatedAt { get; set; } = DateTime.Now;

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
        CreatedAt = room.CreatedAt;
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomInfo"/>
    /// </summary>
    /// <remarks>
    /// Нужен для сериализации
    /// </remarks>
    public RoomInfo() { }
}
