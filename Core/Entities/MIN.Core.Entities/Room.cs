using MIN.Core.Entities.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Entities;

/// <summary>
/// Комната
/// </summary>
public class Room : IRoomData
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Room"/>
    /// </summary>
    public Room(string name = "Неизвестная Комната", int maximumParticipants = 2)
    {
        Name = name;
        MaximumParticipants = maximumParticipants;
    }

    /// <inheritdoc />
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public int MaximumParticipants { get; set; }

    /// <inheritdoc />
    public int ParticipantCount => CurrentParticipants.Count();

    /// <inheritdoc />
    public bool IsActive { get; set; }

    /// <summary>
    /// Хост комнаты
    /// </summary>
    public ParticipantInfo HostParticipant { get; set; } = null!;

    /// <summary>
    /// Получить текущих участников комнаты
    /// </summary>
    public List<ParticipantInfo> CurrentParticipants { get; set; } = [];

    /// <summary>
    /// Получить текущих участников комнаты
    /// </summary>
    public List<IMessage> ChatHistory { get; set; } = [];

    /// <summary>
    /// Заполнена ли комната
    /// </summary>
    public bool IsFull => CurrentParticipants.Count() >= MaximumParticipants;

    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    public void AddParticipant(ParticipantInfo participantInfo)
    {
        CurrentParticipants.Add(participantInfo);
    }

    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    public bool RemoveParticipant(Guid participantId)
    {
        var foundParticipant = CurrentParticipants.FirstOrDefault(x => x.Id == participantId);
        return CurrentParticipants.Remove(foundParticipant!);
    }
}
