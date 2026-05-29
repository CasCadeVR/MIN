using MIN.Sessions.Core.Services.Contracts.Enums;
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
    };
}
