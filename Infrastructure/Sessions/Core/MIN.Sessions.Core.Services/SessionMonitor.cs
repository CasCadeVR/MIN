using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class SessionMonitor : IHostedService
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="SessionMonitor"/>
    /// </summary>
    public SessionMonitor(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        eventBus.Subscribe<ParticipantLeftEvent>(OnParticipantLeft);
        return Task.CompletedTask;
    }

    private async Task OnParticipantLeft(ParticipantLeftEvent e, CancellationToken cancellationToken)
    {
        var activeSubRooms = subRoomManager.GetRoomSubRooms(e.Message.RoomId);
        foreach (var subRoom in activeSubRooms)
        {
            subRoomManager.LeaveSubRoom(e.Message.RoomId, subRoom.Id, e.Message.Participant.Id);
        }
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
