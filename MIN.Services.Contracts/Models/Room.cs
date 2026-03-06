using MIN.Services.Contracts.Models.Messages;

namespace MIN.Services.Contracts.Models;

/// <summary>
/// Комната
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр <see cref="Room"/>
/// </remarks>
public class Room(string name = "Room", int maximumParticipants = 2)
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Название комнаты
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Максимальное количество участников
    /// </summary>
    public int MaximumParticipants { get; set; } = maximumParticipants;

    /// <summary>
    /// Хост комнаты
    /// </summary>
    public virtual Participant HostParticipant { get; set; } = null!;

    /// <summary>
    /// Получить историю чата
    /// </summary>
    public List<ChatMessage> ChatHistory { get; private set; } = new();

    /// <summary>
    /// Получить текущих участников комнаты
    /// </summary>
    public List<Participant> CurrentParticipants { get; private set; } = new();

    /// <summary>
    /// Заполнена ли комната
    /// </summary>
    public bool IsFull => CurrentParticipants.Count >= MaximumParticipants;

    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    public void AddParticipant(Participant participant)
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
    public void AddMessage(ChatMessage message)
        => ChatHistory.Add(message);

    /// <summary>
    /// Обновить информацию о комнате
    /// </summary>
    public void UpdateInfo(Room room)
    {
        Name = room.Name;
        MaximumParticipants = room.MaximumParticipants;
        HostParticipant = room.HostParticipant;
    }
}
