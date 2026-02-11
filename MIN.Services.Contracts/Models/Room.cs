using System.Collections.Generic;

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

    /// <summary>
    /// Текущие участники
    /// </summary>
    private List<Participant> currentParticipants { get; set; } = new List<Participant>();

    /// <summary>
    /// История сообщений
    /// </summary>
    private List<ChatMessage> chatHistory { get; set; } = new List<ChatMessage>();

    // События
    public event Action<Participant>? ParticipantJoined;
    public event Action<Participant>? ParticipantLeft;
    public event Action<ChatMessage>? MessageReceived;
    public event Action<Room>? RoomInfoChanged;

    // Публичные методы для управления комнатой
    public void AddParticipant(Participant participant)
    {
        if (IsFull)
        {
            throw new InvalidOperationException("Достигнуто максимальное количество участников.");
        }

        currentParticipants.Add(participant);
        OnParticipantJoined(participant);
    }

    public void RemoveParticipant(Participant participant)
    {
        if (currentParticipants.Remove(participant))
        {
            OnParticipantLeft(participant);
        }
    }

    public void AddMessage(ChatMessage message)
    {
        chatHistory.Add(message);
        OnMessageReceived(message);
    }

    public void UpdateStats(Room room)
    {
        Name = room.Name;
        MaximumParticipants = room.MaximumParticipants;
        HostParticipant = room.HostParticipant;
        OnRoomInfoChanged(room);
    }

    /// <summary>
    /// Получить историю чата
    /// </summary>
    public List<ChatMessage> ChatHistory => chatHistory.OrderByDescending(x => x.Time).ToList();

    /// <summary>
    /// Получить текущих участников комнаты
    /// </summary>
    public List<Participant> CurrentParticipants => currentParticipants;

    /// <summary>
    /// Заполнена ли комната
    /// </summary>
    public bool IsFull => CurrentParticipants.Count >= MaximumParticipants;

    // Методы для безопасного вызова событий
    protected virtual void OnParticipantJoined(Participant participant)
    {
        ParticipantJoined?.Invoke(participant);
    }

    protected virtual void OnParticipantLeft(Participant participant)
    {
        ParticipantLeft?.Invoke(participant);
    }

    protected virtual void OnMessageReceived(ChatMessage message)
    {
        MessageReceived?.Invoke(message);
    }

    protected virtual void OnRoomInfoChanged(Room room)
    {
        RoomInfoChanged?.Invoke(room);
    }
}
