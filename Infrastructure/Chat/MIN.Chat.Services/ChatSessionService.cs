using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatSessionService"/>
public sealed class ChatSessionService : IChatSessionService
{
    private readonly IMessageRouter messageRouter;
    private readonly ISessionScanner sessionScanner;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatSessionService"/>
    /// </summary>
    public ChatSessionService(IMessageRouter messageRouter,
        ISessionScanner sessionScanner,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.sessionScanner = sessionScanner;
        this.identityService = identityService;
    }

    async Task IChatSessionService.SendSessionHostRequestAsync(Guid roomId, Session selectedSession, CancellationToken cancellationToken)
    {
        if (!sessionScanner.IsSessionInstalled(selectedSession.SessionId))
        {
            throw new DirectoryNotFoundException($"У вас не установлена программа для {selectedSession.Name}");
        }

        await SendAsync(new SessionHostRequestMessage()
        {
            SessionId = selectedSession.SessionId,
            SessionVersion = selectedSession.Version,
        }, roomId, cancellationToken);
    }

    async Task IChatSessionService.SendSessionJoinRequest(Guid roomId, SessionReadyMessage sessionReadyMessage, CancellationToken cancellationToken)
    {
        var session = sessionScanner.GetSessionById(sessionReadyMessage.Session.SessionId);

        if (session == null)
        {
            throw new DirectoryNotFoundException($"У вас не установлена программа для {sessionReadyMessage.Session.Name}");
        }

        await SendAsync(new SessionJoinRequestMessage()
        {
            SessionId = session.SessionId,
            SessionVersion = session.Version,
            SubRoomId = sessionReadyMessage.SubRoomId,
        }, roomId, cancellationToken);
    }

    async Task IChatSessionService.ScanDownloadedSessions(CancellationToken cancellationToken)
    {
        await sessionScanner.ScanAsync(cancellationToken);
    }

    private async Task SendAsync(IMessage? message, Guid roomId, CancellationToken cancellationToken)
    {
        if (message == null)
        {
            throw new NotImplementedException();
        }

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }
}
