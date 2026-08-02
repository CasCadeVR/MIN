using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <summary>
/// Сервис для обработки состояния соединения в комнатах
/// </summary>
public sealed class ConnectionMonitor : IHostedService, IAsyncDisposable
{
    private readonly IRoomConnector roomConnector;
    private readonly IRoomHoster roomHoster;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly IRoomStore roomStore;
    private readonly IRoomFactory roomFactory;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly ILoggerProvider logger;

    private CancellationTokenSource cts = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionMonitor"/>
    /// </summary>
    public ConnectionMonitor(IRoomConnector roomConnector,
        IRoomHoster roomHoster,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        IRoomStore roomStore,
        IRoomFactory roomFactory,
        INetworkErrorHandler networkErrorHandler,
        ILoggerProvider logger)
    {
        this.roomConnector = roomConnector;
        this.roomHoster = roomHoster;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.roomStore = roomStore;
        this.roomFactory = roomFactory;
        this.networkErrorHandler = networkErrorHandler;
        this.logger = logger;
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        roomConnector.ConnectionStateChanged += OnConnectionStateChanged;
        roomHoster.ConnectionStateChanged += OnConnectionStateChanged;
        await Task.CompletedTask;
    }

    private async void OnConnectionStateChanged(object? sender, RoomConnectionStateChangedEventArgs e)
    {
        var roomId = e.RoomId;

        try
        {
            if (!roomStore.RoomExists(roomId))
            {
                return;
            }

            var leavingMessage = e.LeavingMessage;
            var needToDisconnect = false;

            if (!e.IsConnected)
            {
                var context = roomFactory.GetOrCreateContext(roomId);
                if (!context.Connections.TryGetParticipantFromConnectionId(e.ConnectionId, out var leavingParticipant))
                {
                    return;
                }

                var hostParticipantId = roomStore.GetRoomHostParticipantId(roomId);
                var isHostLeaving = hostParticipantId == leavingParticipant.Id; // Для клиента всегда true т.к. он подключен к хосту

                needToDisconnect = isHostLeaving;

                if (isHostLeaving)
                {
                    leavingMessage = !string.IsNullOrEmpty(e.LeavingMessage)
                        ? e.LeavingMessage
                        : "Хост остановил комнату";
                    roomStore.Remove(roomId);
                    roomFactory.DestroyContext(roomId);
                    await eventBus.PublishAsync(new RoomClosedEvent() { RoomId = roomId });
                }
                else if (context.Participants.TryGetParticipantById(leavingParticipant.Id, out _))
                {
                    await HandleConnectionLossAsHost(context, e, hostParticipantId, leavingParticipant);
                }
            }

            await eventBus.PublishAsync(new ConnectionStatusChangedEvent
            {
                RoomId = roomId,
                ConnectionId = e.ConnectionId,
                LeavingMessage = leavingMessage,
                NeedToDisconnect = needToDisconnect,
                IsConnected = e.IsConnected
            }, cts.Token);
        }
        catch (Exception ex)
        {
            logger.Log($"Произошла ошибка во время обработки изменения состояния подключения: {ex.Message}");
        }
    }

    private async Task HandleConnectionLossAsHost(RoomContext context, RoomConnectionStateChangedEventArgs e,
        Guid hostParticipantId, ParticipantInfo leavingParticipant)
    {
        context.Connections.Unregister(e.ConnectionId);
        var participantLeftMessage = new ParticipantLeftMessage()
        {
            Participant = leavingParticipant,
            WasKicked = networkErrorHandler.GetDisconnectDetailsFor(leavingParticipant.Id, e.RoomId) != null
        };

        await messageRouter.RouteAsync(participantLeftMessage, e.RoomId, hostParticipantId, cts.Token);
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (cts != null)
        {
            try
            {
                cts?.Cancel();
            }
            catch (ObjectDisposedException) { }
            cts?.Dispose();
            cts = null!;
        }
        roomConnector.ConnectionStateChanged -= OnConnectionStateChanged;
        roomHoster.ConnectionStateChanged -= OnConnectionStateChanged;
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await StopAsync();
}
