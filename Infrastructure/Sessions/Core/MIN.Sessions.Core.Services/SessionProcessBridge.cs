using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Events;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class SessionProcessBridge : IHostedService
{
    private readonly ISubRoomManager subRoomManager;
    private readonly IEventBus eventBus;
    private readonly IMessageRouter messageRouter;
    private readonly ISessionProcessTransport processTransport;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="SessionMonitor"/>
    /// </summary>
    public SessionProcessBridge(ISubRoomManager subRoomManager,
        IEventBus eventBus,
        IMessageRouter messageRouter,
        ISessionProcessTransport processTransport,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.subRoomManager = subRoomManager;
        this.eventBus = eventBus;
        this.messageRouter = messageRouter;
        this.processTransport = processTransport;
        this.identityService = identityService;
        this.logger = logger;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        processTransport.MessageReceived += OnTransportMessage;
        return Task.CompletedTask;
    }

    private async void OnTransportMessage(object? sender, ProcessTransportMessageEventArgs e)
    {

    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
