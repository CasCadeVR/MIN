using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Chess.Services.Contracts.Services;

/// <summary>
/// Провайдер информации о сессии для шахмат
/// </summary>
public static class ChessSessionProvider
{
    /// <summary>
    /// Получить информацию о сессии шахмат
    /// </summary>
    public static Session GetChessSession() => new()
    {
        SessionType = SessionType.Chess,
        Name = "Шахматы",
        Description = "Сыграй с друзьями в шахматы!",
    };
}
