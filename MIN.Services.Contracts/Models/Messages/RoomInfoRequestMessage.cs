namespace MIN.Services.Contracts.Models.Messages;

/// <summary>
/// Запрос информации о комнате
/// </summary>
public class RoomInfoRequestMessage()
{
    /// <summary>
    /// Название комнаты
    /// </summary>
    public string? RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Максимальное количество участников
    /// </summary>
    public int? MaxParticipants { get; set; }
}
