namespace MIN.Services.Contracts.Models;

public class RoomInfoMessage
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Название комнаты
    /// </summary>
    public string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Максимальное количество участников
    /// </summary>
    public int MaxParticipants { get; set; }

    /// <summary>
    /// Имя хоста
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// Имя компьютера хоста
    /// </summary>
    public string HostPCName { get; set; } = string.Empty;

    /// <summary>
    /// Текущие участники
    /// </summary>
    public List<Participant> CurrentParticipants { get; set; } = new();

    /// <summary>
    /// История сообщений
    /// </summary>
    public List<ChatMessage> ChatHistory { get; set; } = new();
}
