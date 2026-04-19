using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Events;
using MIN.Core.Messaging.RoomRelated.ParticipantRelated;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Rooms;

/// <summary>
/// Сервис для обработки состояния соединения в комнатах
/// </summary>
public sealed class ConnectionMonitor : IHostedService, IAsyncDisposable
{
    private readonly ITransport transport;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly IRoomStore roomStore;
    private readonly IParticipantStore participantStore;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly ILoggerProvider logger;

    private CancellationTokenSource cts = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionMonitor"/>
    /// </summary>
    public ConnectionMonitor(ITransport transport,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        IRoomStore roomStore,
        IParticipantStore participantStore,
        IParticipantConnectionRegistry participantConnectionRegistry,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.roomStore = roomStore;
        this.participantStore = participantStore;
        this.participantConnectionRegistry = participantConnectionRegistry;
        this.logger = logger;
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        transport.ConnectionStateChanged += OnConnectionStateChanged;
        await Task.CompletedTask;
    }

    private async void OnConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        try
        {
            if (!roomStore.RoomExists(e.RoomId))
            {
                return;
            }

            var leavingMessage = e.LeavingMessage;
            var needToDisconnect = false;

            if (!e.IsConnected)
            {
                participantConnectionRegistry.TryGetParticipantFromConnectionId(e.RoomId, e.ConnectionId, out var leavingParticipant);

                var hostParticipantId = roomStore.GetRoomHostParticipantId(e.RoomId);
                var isHostLeaving = hostParticipantId == leavingParticipant.Id;

                needToDisconnect = isHostLeaving;

                if (isHostLeaving)
                {
                    leavingMessage = !string.IsNullOrEmpty(e.LeavingMessage) ? leavingMessage : "Хост остановил комнату";
                }
                else if (participantStore.TryGetParticipantById(e.RoomId, leavingParticipant.Id, out _))
                {
                    participantConnectionRegistry.Unregister(e.RoomId, e.ConnectionId);
                    var participantLeftMessage = new ParticipantLeftMessage()
                    {
                        Participant = leavingParticipant,
                        RoomId = e.RoomId,
                    };

                    await messageRouter.RouteAsync(participantLeftMessage, e.RoomId, hostParticipantId, cts.Token);
                }
            }

            await eventBus.PublishAsync(new ConnectionStatusChangedEvent
            {
                RoomId = e.RoomId,
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

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (cts != null)
        {
            await cts.CancelAsync();
            cts.Dispose();
            cts = null!;
        }
        transport.ConnectionStateChanged -= OnConnectionStateChanged;
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await StopAsync();
}
