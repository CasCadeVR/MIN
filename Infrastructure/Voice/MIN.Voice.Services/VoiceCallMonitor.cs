using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Enums;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Voice.Events;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Services;

/// <summary>
/// Сервис для отслеживания состояния звонков
/// </summary>
public class VoiceCallMonitor : IHostedService
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly IMuteService muteService;
    private readonly IVoicePlaybackService voicePlaybackService;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="VoiceCallMonitor"/>
    /// </summary>
    public VoiceCallMonitor(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        IMuteService muteService,
        IVoicePlaybackService voicePlaybackService,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.muteService = muteService;
        this.voicePlaybackService = voicePlaybackService;
        this.identityService = identityService;
        this.logger = logger;
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        eventBus.Subscribe<VoiceCallEstablishedEvent>(OnVoiceCallEstablished);
        eventBus.Subscribe<VoiceCallLeftEvent>(OnVoiceCallLeft);
        eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft);
    }

    private async Task OnVoiceCallEstablished(VoiceCallEstablishedEvent e, CancellationToken cancellationToken)
    {
        voicePlaybackService.RegisterSubroomVoice(e.SubRoomId);
        await muteService.UnmuteSelf(e.RoomId, e.SubRoomId, cancellationToken);
    }

    private async Task OnVoiceCallLeft(VoiceCallLeftEvent e, CancellationToken cancellationToken)
    {
        voicePlaybackService.UnregisterSubroomVoice(e.SubRoomId);
        await muteService.MuteSelf(e.RoomId, e.SubRoomId, cancellationToken);
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent e, CancellationToken cancellationToken)
    {
        var roomId = e.RoomId;
        var participantId = e.Message.Participant.Id;

        var activeSubRooms = subRoomManager.GetRoomSubRooms(roomId).Where(x => x.Purpose == SubRoomPurpose.Voice && x.IsActive);
        foreach (var subRoom in activeSubRooms)
        {
            if (subRoomManager.IsInSubRoom(roomId, subRoom.Id, participantId))
            {
                var isLast = !subRoomManager.LeaveSubRoom(roomId, subRoom.Id, participantId);

                await messageRouter.RouteAsync(new VoiceParticipantLeftMessage()
                {
                    SubRoomId = subRoom.Id,
                    Participant = e.Message.Participant,
                }, roomId, identityService.SelfParticipant.Id, cancellationToken);

                if (isLast)
                {
                    await messageRouter.RouteAsync(new VoiceCallEndedMessage()
                    {
                        SubRoomId = subRoom.Id,
                    }, roomId, identityService.SelfParticipant.Id, cancellationToken);
                }
            }
        }
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
