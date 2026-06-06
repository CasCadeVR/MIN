using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Events;
using MIN.Sessions.Core.Messaging;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class SessionMonitor : IHostedService
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly ISessionProcessInitializer sessionProcessInitializer;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="SessionMonitor"/>
    /// </summary>
    public SessionMonitor(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        ISessionProcessInitializer sessionProcessInitializer,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.sessionProcessInitializer = sessionProcessInitializer;
        this.identityService = identityService;
        this.logger = logger;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        eventBus.Subscribe<JoinResponseReceivedEvent>(OnJoinResponseReceived);
        eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft);
        eventBus.Subscribe<SessionDeactivatedEvent>(OnSessionDeactivated);
        return Task.CompletedTask;
    }

    private async Task OnJoinResponseReceived(JoinResponseReceivedEvent e, CancellationToken cancellationToken)
    {
        var hostResult = await sessionProcessInitializer.StartAsync(e.RoomId, e.SubRoomId,
            e.Session.ClientPath, SessionProcessRole.Client, cancellationToken);

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
        await sessionProcessInitializer.StopAsync(e.RoomId, e.SubRoomId);
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent e, CancellationToken cancellationToken)
    {
        var roomId = e.Message.RoomId;
        var participantId = e.Message.Participant.Id;

        var activeSubRooms = subRoomManager.GetRoomSubRooms(roomId);
        foreach (var subRoom in activeSubRooms)
        {
            if (subRoomManager.IsInSubRoom(roomId, subRoom.Id, participantId))
            {
                subRoomManager.LeaveSubRoom(roomId, subRoom.Id, participantId);

                var leaveMessage = new SessionParticipantLeftMessage()
                {
                    SubRoomId = subRoom.Id,
                    Participant = e.Message.Participant,
                };

                await messageRouter.RouteAsync(leaveMessage, roomId, identityService.SelfParticipant.Id, cancellationToken);
            }
        }
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
