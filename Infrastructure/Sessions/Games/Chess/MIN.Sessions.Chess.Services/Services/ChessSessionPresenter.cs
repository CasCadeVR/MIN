using MIN.Sessions.Chess.Services.Contracts.Services;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Chess.Services.Services;

/// <summary>
/// Представление сессии о шахматах
/// </summary>
public class ChessSessionPresenter : ISessionPresenter
{
    Session ISessionPresenter.GetSession() => ChessSessionProvider.GetChessSession();
}
