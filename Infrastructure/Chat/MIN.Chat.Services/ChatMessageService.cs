using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatMessageService"/>
public sealed class ChatMessageService : IChatMessageService
{
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatMessageService"/>
    /// </summary>
    public ChatMessageService(IMessageRouter messageRouter,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.identityService = identityService;
    }

    async Task IChatMessageService.DeleteMessageAsync(Guid roomId, Guid messageId, CancellationToken cancellationToken)
        => await messageRouter.RouteAsync(new ChatDeleteMessage
        {
            MessageIdToDelete = messageId,
        }, roomId, identityService.SelfParticipant.Id, cancellationToken);
}
