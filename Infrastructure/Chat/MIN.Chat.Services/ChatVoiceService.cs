using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Voice.Events;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatVoiceService"/>
public sealed class ChatVoiceService : IChatVoiceService
{
    private readonly IVoiceCallStateService voiceCallStateService;
    private readonly IMessageRouter messageRouter;
    private readonly IEventBus eventBus;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatVoiceService"/>
    /// </summary>
    public ChatVoiceService(IVoiceCallStateService voiceCallStateService,
        IMessageRouter messageRouter,
        IEventBus eventBus,
        IIdentityService identityService)
    {
        this.voiceCallStateService = voiceCallStateService;
        this.messageRouter = messageRouter;
        this.eventBus = eventBus;
        this.identityService = identityService;
    }

    async Task IChatVoiceService.RequestCallStateAsync(Guid roomId, CancellationToken cancellationToken)
        => await SendAsync(new VoiceCallStateRequestMessage(), roomId, cancellationToken);

    async Task IChatVoiceService.StartCallAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var voiceCallContext = voiceCallStateService.GetRoomVoiceCallContext();

        if (voiceCallContext != null && voiceCallContext.Value.RoomId != roomId)
        {
            await LeaveCallAsync(voiceCallContext.Value.RoomId, voiceCallContext.Value.SubRoomId, cancellationToken);
        }

        await SendAsync(new VoiceCallStartRequestMessage(), roomId, cancellationToken);
    }

    async Task IChatVoiceService.JoinCallAsync(Guid roomId, int subRoomId, CancellationToken cancellationToken)
    {
        var voiceCallContext = voiceCallStateService.GetRoomVoiceCallContext();

        if (voiceCallContext != null && voiceCallContext.Value.RoomId != roomId)
        {
            await LeaveCallAsync(voiceCallContext.Value.RoomId, voiceCallContext.Value.SubRoomId, cancellationToken);
        }

        await SendAsync(new VoiceCallJoinRequestMessage()
        {
            SubRoomId = subRoomId,
        }, roomId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task LeaveCallAsync(Guid roomId, int subRoomId, CancellationToken cancellationToken)
    {
        await eventBus.PublishAsync(new VoiceCallLeftEvent()
        {
            RoomId = roomId,
            SubRoomId = subRoomId,
        }, cancellationToken);

        await SendAsync(new VoiceCallLeaveMessage()
        {
            SubRoomId = subRoomId,
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
