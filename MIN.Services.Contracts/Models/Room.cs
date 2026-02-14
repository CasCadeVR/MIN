namespace MIN.Services.Contracts.Models;

/// <summary>
/// Комната
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр <see cref="Room"/>
/// </remarks>
public class Room(string name = "Room", int maximumClients = 2)
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
    public int MaximumParticipants { get; set; } = maximumClients;

    /// <summary>
    /// Хост комнаты
    /// </summary>
    public virtual Participant HostParticipant { get; set; } = null!;

    private List<ChatMessage> chatHistory { get; set; } = new();

    /// <summary>
    /// Получить историю чата
    /// </summary>
    public List<ChatMessage> ChatHistory => chatHistory.OrderByDescending(x => x.Time).ToList();

    private List<Participant> currentParticipants { get; set; } = new();

    /// <summary>
    /// Получить текущих участников комнаты
    /// </summary>
    public List<Participant> CurrentParticipants => currentParticipants;

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

        currentParticipants.Add(participant);
    }

    /// <summary>
    /// Убрать участника из комнаты
    /// </summary>
    public bool RemoveParticipant(Participant participant)
        =>currentParticipants.Remove(participant);

    /// <summary>
    /// Добавить сообщение
    /// </summary>
    public void AddMessage(ChatMessage message)
        => chatHistory.Add(message);
    
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
