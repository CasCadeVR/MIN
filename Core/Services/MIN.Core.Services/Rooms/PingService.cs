using System.Collections.Concurrent;
using System.Diagnostics;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.Stateless.RoomRelated.Ping;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Models;

namespace MIN.Core.Services.Rooms;

/// <inheritdoc cref="IPingService"/>
public class PingService : IPingService, IDisposable
{
    private readonly IMessageSender messageSender;
    private readonly IEventBus eventBus;

    private const int ConnectionTimeoutSeconds = 60;
    private const int PingIntervalMs = 3_000;

    private readonly ConcurrentDictionary<PingContext, DateTime> lastPingSeen = new(); // pingContext / missed pong count
    private readonly ConcurrentDictionary<PingContext, Stopwatch> pingTravel = new(); // pingContext / pingTimer
    private readonly System.Timers.Timer pingTimer;

    /// <inheritdoc />
    public event Func<Guid, Guid, Task>? OnConnectionTimeout;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="PingService"/>
    /// </summary>
    public PingService(IEventBus eventBus,
        IMessageSender messageSender)
    {
        this.messageSender = messageSender;
        this.eventBus = eventBus;

        pingTimer = new System.Timers.Timer
        {
            Interval = PingIntervalMs
        };
        pingTimer.Elapsed += PingTimer_Elapsed;

        SubscribeToEvents();
    }

    private async void PingTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        var now = DateTime.Now;

        foreach (var (context, lastSeen) in lastPingSeen)
        {
            if ((now - lastSeen).Seconds > ConnectionTimeoutSeconds)
            {
                OnConnectionTimeout?.Invoke(context.RoomId, context.ConnectionId);
            }
            else if (context.Role == Role.Client)
            {
                if (pingTravel.TryGetValue(context, out var elapsedStopwatch))
                {
                    await eventBus.PublishAsync(new PingMeasuredEvent()
                    {
                        RoomId = context.RoomId,
                        PingMs = (int)elapsedStopwatch.ElapsedMilliseconds,
                    });
                }
                else
                {
                    var stopwatchPing = new Stopwatch();
                    stopwatchPing.Start();
                    pingTravel[context] = stopwatchPing;
                    await messageSender.SendAsync(new PingMessage(), context.RoomId, context.ConnectionId);
                }
            }
        }
    }

    private void SubscribeToEvents()
    {
        eventBus.Subscribe<PingPongReceivedEvent>(OnPingPongReceived);
    }

    private async Task OnPingPongReceived(PingPongReceivedEvent e, CancellationToken cancellationToken)
    {
        var context = new PingContext(e.Role, e.RoomId, e.ConnectionId);

        if (lastPingSeen.ContainsKey(context))
        {
            lastPingSeen[context] = DateTime.Now;

            if (e.Role == Role.Client)
            {
                if (pingTravel.TryRemove(context, out var elapsedStopwatch))
                {
                    elapsedStopwatch.Stop();
                    await eventBus.PublishAsync(new PingMeasuredEvent()
                    {
                        RoomId = e.RoomId,
                        PingMs = (int)elapsedStopwatch.ElapsedMilliseconds,
                    }, cancellationToken);
                }
            }
        }
    }

    Task IPingService.RegisterHeartbeatSession(Role role, Guid roomId, Guid connectionId)
    {
        if (lastPingSeen.IsEmpty)
        {
            pingTimer.Start();
        }
        lastPingSeen.TryAdd(new PingContext(role, roomId, connectionId), DateTime.Now);
        return Task.CompletedTask;
    }

    Task IPingService.UnregisterHeartbeatSession(Role role, Guid roomId, Guid connectionId)
    {
        if (lastPingSeen.Count - 1 == 0)
        {
            pingTimer.Stop();
        }
        lastPingSeen.Remove(new PingContext(role, roomId, connectionId), out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    void IDisposable.Dispose()
    {
        pingTimer.Stop();
        pingTimer.Dispose();
    }
}
