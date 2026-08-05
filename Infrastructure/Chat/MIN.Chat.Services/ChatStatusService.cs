using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatStatusService"/>
public sealed class ChatStatusService : IChatStatusService
{
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatStatusService"/>
    /// </summary>
    public ChatStatusService(IMessageRouter messageRouter,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.identityService = identityService;
    }

    async Task IChatStatusService.SendSelfOnlineStatusChangedAsync(Guid roomId, OnlineStatus newStatus, CancellationToken cancellationToken)
    {
        var message = new OnlineStatusChangedMessage
        {
            Status = newStatus,
        };

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }
}
