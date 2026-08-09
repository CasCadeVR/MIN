using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Voice.Messaging;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatVoiceService"/>
public sealed class ChatVoiceService : IChatVoiceService
{
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatVoiceService"/>
    /// </summary>
    public ChatVoiceService(IMessageRouter messageRouter,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.identityService = identityService;
    }

    async Task IChatVoiceService.RequestCallStateAsync(Guid roomId, CancellationToken cancellationToken)
        => await SendAsync(new VoiceCallStateRequestMessage(), roomId, cancellationToken);

    async Task IChatVoiceService.StartCallAsync(Guid roomId, CancellationToken cancellationToken)
        => await SendAsync(new VoiceCallStartRequestMessage(), roomId, cancellationToken);

    async Task IChatVoiceService.JoinCallAsync(Guid roomId, int subRoomId, CancellationToken cancellationToken)
        => await SendAsync(new VoiceCallJoinRequestMessage()
        {
            SubRoomId = subRoomId,
        }, roomId, cancellationToken);

    async Task IChatVoiceService.LeaveCallAsync(Guid roomId, int subRoomId, CancellationToken cancellationToken)
        => await SendAsync(new VoiceCallLeaveMessage()
        {
            SubRoomId = subRoomId,
        }, roomId, cancellationToken);

    private async Task SendAsync(IMessage? message, Guid roomId, CancellationToken cancellationToken)
    {
        if (message == null)
        {
            throw new NotImplementedException();
        }

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }
}
