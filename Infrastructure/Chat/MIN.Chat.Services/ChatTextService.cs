using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatTextService"/>
public sealed class ChatTextService : IChatTextService
{
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatTextService"/>
    /// </summary>
    public ChatTextService(IMessageRouter messageRouter,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.identityService = identityService;
    }

    async Task IChatTextService.SendMessageAsync(Guid roomId, string content, Guid? recipientId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Сообщение не должно быть пустым", nameof(content));
        }

        var message = new ChatTextMessage
        {
            Sender = identityService.SelfParticipant.ToParticipantInfo(),
            Content = content,
            RecipientId = recipientId,
        };

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }
}
