using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для работы с сессиями в чате
/// </summary>
public interface IChatSessionService
{
    /// <summary>
    /// Отправить запрос на хостинг сессии
    /// </summary>
    Task SendSessionHostRequestAsync(Guid roomId, Session selectedSession, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запросить вход в сессию
    /// </summary>
    Task SendSessionJoinRequest(Guid roomId, SessionReadyMessage sessionReadyMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Просканировать загруженные сессии
    /// </summary>
    Task ScanDownloadedSessions(CancellationToken cancellationToken = default);
}
