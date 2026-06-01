using MIN.Desktop.Properties;
using MIN.Sessions.Core.Services.Contracts.Enums;

namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Помошник в загрузке изображений для типа сессии
/// </summary>
public static class SessionImageProvider
{
    /// <summary>
    /// Загрузить сообщение по типу сессии
    /// </summary>
    public static Image LoadImageOutOfSessionType(SessionType sessionType)
        => sessionType switch
        {
            SessionType.Chess => Resources.chesslogo,
            _ => throw new ArgumentOutOfRangeException(nameof(SessionType), $"Not expected sessionType value: {sessionType}"),
        };
}
