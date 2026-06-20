using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
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
        var session = sessionReadyMessage.Session;

        if (!sessionScanner.IsSessionInstalled(session.SessionId))
        {
            throw new DirectoryNotFoundException($"У вас не установлена программа для {session.Name}");
        }

        await SendAsync(new SessionJoinRequestMessage()
        {
            SessionId = session.SessionId,
            SessionVersion = session.Version,
            SubRoomId = sessionReadyMessage.SubRoomId,
        }, roomId, cancellationToken);
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
