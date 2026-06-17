using MIN.Sessions.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Chess.Services.Services;

/// <summary>
/// Представление сессии о шахматах
/// </summary>
public class ChessSessionPresenter : ISessionPresenter
{
    Session ISessionPresenter.GetSession() => new()
    {
        SessionType = SessionType.Chess,
        Name = "Шахматы",
        Description = "Сыграй с друзьями в шахматы!",
        //ServerPath = "Sessions/Chess/MIN.Chess.Server.exe",
        //ClientPath = "Sessions/Chess/MIN.Chess.Client.exe",
        ClientPath = "C:\\Users\\Admin\\Documents\\CSharpProjects\\Learning\\Projects\\MinChess\\Network\\MIN.Chess.Client\\bin\\Debug\\net8.0-windows\\win-x64\\MIN.Chess.Client.exe",
        ServerPath = "C:\\Users\\Admin\\Documents\\CSharpProjects\\Learning\\Projects\\MinChess\\Network\\MIN.Chess.Server\\bin\\Debug\\net8.0-windows\\MIN.Chess.Server.exe",
    };
}
