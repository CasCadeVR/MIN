using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Chess.Messaging.Default;
using MIN.Sessions.Chess.Services.Contracts.Models;
using MIN.Sessions.Core.Messaging;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatSessionService"/>
public sealed class ChatSessionService : IChatSessionService
{
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatSessionService"/>
    /// </summary>
    public ChatSessionService(IMessageRouter messageRouter,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.identityService = identityService;
    }

    async Task IChatSessionService.SendSessionRequestAsync(Guid roomId, Session selectedSession, ISessionHostRequestOptions? hostRequestOptions, CancellationToken cancellationToken)
    {
        IMessage? message = null;

        switch (selectedSession.SessionType)
        {
            case SessionType.Chess:
                ChessHostRequestOptions? chessOptions = null;

                if (hostRequestOptions != null)
                {
                    if (hostRequestOptions is not ChessHostRequestOptions chessHostRequestOptions)
                    {
                        throw new ArgumentException("Несоответсвие типов параметров хостинга");
                    }
                    else
                    {
                        chessOptions = chessHostRequestOptions;
                    }
                }

                message = new ChessHostRequestMessage()
                {
                    Options = chessOptions,
                };
                break;
        }

        await SendAsync(message, roomId, cancellationToken);
    }

    async Task IChatSessionService.SendSessionJoinRequest(Guid roomId, SessionReadyMessage sessionReadyMessage, ISessionJoinRequestOptions? joinRequestOptions, CancellationToken cancellationToken)
    {
        IMessage? message = null;

        switch (sessionReadyMessage.Session.SessionType)
        {
            case SessionType.Chess:
                ChessJoinRequestOptions? chessOptions = null;

                if (joinRequestOptions != null)
                {
                    if (joinRequestOptions is not ChessJoinRequestOptions chessJoinRequestOptions)
                    {
                        throw new ArgumentException("Несоответсвие типов параметров хостинга");
                    }
                    else
                    {
                        chessOptions = chessJoinRequestOptions;
                    }
                }

                message = new ChessJoinRequestMessage()
                {
                    SubRoomId = sessionReadyMessage.SubRoomId,
                    Options = chessOptions,
                };
                break;
        }

        await SendAsync(message, roomId, cancellationToken);
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
