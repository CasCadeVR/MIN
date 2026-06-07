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
        //ServerPath = "Sessions/Chess/MIN.Chess.Server.exe",
        //ClientPath = "Sessions/Chess/MIN.Chess.Client.exe",
        ClientPath = "C:\\Users\\Admin\\Documents\\CSharpProjects\\Learning\\Projects\\MinChess\\Network\\MIN.Chess.Client\\bin\\Debug\\net8.0-windows\\win-x64\\MIN.Chess.Client.exe",
        ServerPath = "C:\\Users\\Admin\\Documents\\CSharpProjects\\Learning\\Projects\\MinChess\\Network\\MIN.Chess.Server\\bin\\Debug\\net8.0\\win-x64\\MIN.Chess.Server.exe",
    };
}
