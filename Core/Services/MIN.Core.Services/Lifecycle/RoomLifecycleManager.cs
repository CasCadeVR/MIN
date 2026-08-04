using MIN.Common.Core.Extensions;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Protocol.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Services.Contracts.Models;
using MIN.Core.Services.Services;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Lifecycle;

/// <inheritdoc cref="IRoomLifecycleManager"/>
public sealed class RoomLifecycleManager : IRoomLifecycleManager
{
    private readonly ITransport transport;
    private readonly IPingService pingService;
    private readonly IRoomConnectionRegistry registry;
    private readonly IEventBus eventBus;
    private readonly ClientRoomService clientService;
    private readonly HostRoomService hostService;

    /// <inheritdoc />
    public event EventHandler<RoomRawMessageReceivedEventArgs>? RawMessageReceived;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomLifecycleManager"/>
    /// </summary>
    public RoomLifecycleManager(ITransport transport,
        IClientHandshake clientHandshake,
        IHostHandshake hostHandshake,
        IRoomStore roomStore,
        IRoomFactory roomFactory,
        IMessageSender messageSender,
        IMessageRouter messageRouter,
        IIdentityService identityService,
        IMessageEncryptor encryptor,
        IPingService pingService,
        IRoomConnectionRegistry registry,
        IVersionProvider versionProvider,
        ISubRoomManager subRoomManager,
        IEventBus eventBus,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.pingService = pingService;
        this.registry = registry;
        this.eventBus = eventBus;

        clientService = new ClientRoomService(transport, clientHandshake, roomStore, roomFactory,
            messageSender, identityService, encryptor, pingService, registry, versionProvider, eventBus, logger);

        hostService = new HostRoomService(roomFactory, hostHandshake, transport, roomStore, eventBus,
            subRoomManager, pingService, registry, identityService, messageRouter, logger);

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        transport.RawMessageReceived += Transport_RawMessageReceived;
        transport.ConnectionStateChanged += Transport_ConnectionStateChanged;
        pingService.OnConnectionTimeout += PingService_OnConnectionTimeout;
    }

    private void Transport_RawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
    {
        if (!clientService.TryResolveRoom(e, out var roomId) && !hostService.TryResolveRoom(e, out roomId))
        {
            return;
        }

        RawMessageReceived?.Invoke(this, new RoomRawMessageReceivedEventArgs(roomId, e));
    }

    private async void Transport_ConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        Role role;

        if (clientService.TryResolveRoom(e, out var roomId))
        {
            role = Role.Client;
        }
        else if (hostService.TryResolveRoom(e, out roomId))
        {
            role = Role.Host;
        }
        else
        {
            return;
        }

        if (e.IsConnected)
        {
            if (role == Role.Host && !await hostService.HandleConnectionConnectedAsync(roomId, e))
            {
                return;
            }

            await PublishConnectionStatusAsync(roomId, e, needToDisconnect: false);
            return;
        }

        var needToDisconnect = role == Role.Client
            ? await clientService.HandleConnectionLostAsync(roomId, e)
            : await hostService.HandleConnectionLostAsync(roomId, e);

        await PublishConnectionStatusAsync(roomId, e, needToDisconnect);
    }

    private async Task PingService_OnConnectionTimeout(Guid roomId, Guid connectionId)
    {
        if (registry.IsHosting(roomId))
        {
            await hostService.HandleConnectionTimeoutAsync(roomId, connectionId);
        }
        else if (registry.IsConnected(roomId))
        {
            await clientService.HandleConnectionTimeoutAsync(roomId, connectionId);
        }
    }

    private async Task PublishConnectionStatusAsync(Guid roomId, ConnectionStateChangedEventArgs e, bool needToDisconnect)
        => await eventBus.PublishAsync(new ConnectionStatusChangedEvent
        {
            RoomId = roomId,
            ConnectionId = e.ConnectionId,
            LeavingMessage = e.DisconnectReason.GetDescription(),
            NeedToDisconnect = needToDisconnect,
            IsConnected = e.IsConnected
        });

    /// <inheritdoc />
    public async Task<ConnectionResult> ConnectAsync(IEndpoint endpoint, CancellationToken cancellationToken = default)
        => await clientService.ConnectAsync(endpoint, cancellationToken);

    /// <inheritdoc />
    public async Task DisconnectAsync(Guid roomId, Guid connectionId, DisconnectReason reason)
        => await clientService.DisconnectAsync(roomId, connectionId, reason);

    /// <inheritdoc />
    public async Task<Room> StartHostingAsync(RoomInfo roomInfo, NetworkOptions networkOptions, CancellationToken cancellationToken = default)
        => await hostService.StartHostingAsync(roomInfo, networkOptions, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<IEndpoint>> UpdateNetworkOptions(Guid roomId, NetworkOptions newNetworkOptions, CancellationToken cancellationToken = default)
        => await hostService.UpdateNetworkOptions(roomId, newNetworkOptions, cancellationToken);

    /// <inheritdoc />
    public async Task StopHostingAsync(Guid roomId)
        => await hostService.StopHostingAsync(roomId);

    /// <inheritdoc />
    public async Task KickClientAsync(Guid roomId, Guid participantId, DisconnectReason reason)
        => await hostService.KickClientAsync(roomId, participantId, reason);
}
