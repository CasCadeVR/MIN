using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class SessionMonitor : IHostedService
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly ISessionProcessManager sessionProcessManager;
    private readonly ISessionProcessBridge sessionProcessBridge;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="SessionMonitor"/>
    /// </summary>
    public SessionMonitor(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        ISessionProcessManager sessionProcessManager,
        ISessionProcessBridge sessionProcessBridge,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.sessionProcessManager = sessionProcessManager;
        this.sessionProcessBridge = sessionProcessBridge;
        this.identityService = identityService;
        this.logger = logger;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        sessionProcessBridge.StartListeningAsync(cancellationToken);
        eventBus.Subscribe<RoomClosedEvent>(OnRoomClosed);
        eventBus.Subscribe<JoinResponseReceivedEvent>(OnJoinResponseReceived);
        eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft);
        eventBus.Subscribe<SessionDeactivatedEvent>(OnSessionDeactivated);
        return Task.CompletedTask;
    }

    private async Task OnRoomClosed(RoomClosedEvent e, CancellationToken cancellationToken)
    {
        await sessionProcessManager.StopForRoomAsync(e.RoomId);
    }

    private async Task OnJoinResponseReceived(JoinResponseReceivedEvent e, CancellationToken cancellationToken)
    {
        var hostResult = await sessionProcessManager.StartAsync(e.Session.ClientPath,
            new ProcessContext(e.RoomId, e.SubRoomId, SessionProcessRole.Client), cancellationToken);

        if (hostResult == false)
        {
            await eventBus.PublishAsync(new ErrorOccurredEvent()
            {
                ErrorMessage = $"У вас повреждёна или утеряна программа для {e.Session.Name}"
            }, cancellationToken);
        }
    }

    private async Task OnSessionDeactivated(SessionDeactivatedEvent e, CancellationToken cancellationToken)
    {
        await sessionProcessManager.StopAsync(new ProcessContext(e.RoomId, e.SubRoomId, SessionProcessRole.Server));
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent e, CancellationToken cancellationToken)
    {
        var roomId = e.RoomId;
        var participantId = e.Message.Participant.Id;

        var activeSubRooms = subRoomManager.GetRoomSubRooms(roomId);
        foreach (var subRoom in activeSubRooms)
        {
            if (subRoomManager.IsInSubRoom(roomId, subRoom.Id, participantId))
            {
                var isLast = !subRoomManager.LeaveSubRoom(roomId, subRoom.Id, participantId);

                var leaveMessage = new SessionParticipantLeftMessage()
                {
                    SubRoomId = subRoom.Id,
                    Participant = e.Message.Participant,
                    IsLast = isLast
                };

                await messageRouter.RouteAsync(leaveMessage, roomId, identityService.SelfParticipant.Id, cancellationToken);
            }
        }
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        sessionProcessBridge.StopListeningAsync(cancellationToken);
        sessionProcessManager.StopAllAsync();
        return Task.CompletedTask;
    }
}
