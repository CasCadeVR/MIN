namespace MIN.Sessions.Core.Messaging.Contracts.Enums;

/// <summary>
/// Направление сообщения сессии
/// </summary>
public enum SessionMessageRoute : int
{
    /// <summary>
    /// Всегда через сервер (по умолчанию — безопасно)
    /// </summary>
    ViaServer = 0,

    /// <summary>
    /// Сразу всем участникам подкомнаты (для PositionUpdate и т.п.)
    /// </summary>
    Direct = 1,
}
