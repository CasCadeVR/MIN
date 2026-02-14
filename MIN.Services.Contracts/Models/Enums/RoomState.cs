namespace MIN.Services.Contracts.Models.Enums;

/// <summary>
/// Состояние комнаты
/// </summary>
public enum RoomState
{
    /// <summary>
    /// Создан
    /// </summary>
    Created = 0,

    /// <summary>
    /// Вошёл
    /// </summary>
    Joined = 1,

    /// <summary>
    /// Отключен
    /// </summary>
    Disconnected = 2,

    /// <summary>
    /// Ошибка
    /// </summary>
    Error = 3,
}
