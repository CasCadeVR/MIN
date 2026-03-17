using MIN.Entities.Contracts;
using MIN.Messaging.Contracts.Messages;

namespace MIN.Entities;

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
    public int ParticipantCount => CurrentParticipants.Count;

    /// <inheritdoc />
    public bool IsActive { get; set; }

    /// <summary>
    /// Хост комнаты
    /// </summary>
    public IParticipantData HostParticipant { get; set; } = null!;

    /// <summary>
    /// Получить историю чата
    /// </summary>
    public List<IMessage> ChatHistory { get; private set; } = new();

    /// <summary>
    /// Получить текущих участников комнаты
    /// </summary>
    public List<IParticipantData> CurrentParticipants { get; private set; } = new();

    /// <summary>
    /// Заполнена ли комната
    /// </summary>
    public bool IsFull => CurrentParticipants.Count >= MaximumParticipants;

    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    public void AddParticipant(IParticipantData participant)
    {
        if (IsFull)
        {
            throw new InvalidOperationException("Достигнуто максимальное количество участников.");
        }

        CurrentParticipants.Add(participant);
    }

    /// <summary>
    /// Убрать участника из комнаты
    /// </summary>
    public bool RemoveParticipantById(Guid id)
    {
        var foundParticipant = CurrentParticipants.Where(x => x.Id == id).FirstOrDefault();
        return CurrentParticipants.Remove(foundParticipant!);
    }

    /// <summary>
    /// Добавить сообщение
    /// </summary>
    public void AddMessage(IMessage message)
        => ChatHistory.Add(message);

    /// <summary>
    /// Обновить информацию о комнате
    /// </summary>
    public void UpdateInfo(IRoomData room)
    {
        Name = room.Name;
        MaximumParticipants = room.MaximumParticipants;
    }

    /// <summary>
    /// Деактивировать комнату
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Активировать комнату
    /// </summary>
    public void Activate() => IsActive = true;
}
