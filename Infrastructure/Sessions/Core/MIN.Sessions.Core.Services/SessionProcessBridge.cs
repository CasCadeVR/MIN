using System.Text;
using System.Text.Json;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.Contracts.Models;
using MIN.Sessions.Core.Messaging.Ipc;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Serialization.Contracts;
using MIN.Sessions.Core.Transport.Contracts.Enums;
using MIN.Sessions.Core.Transport.Contracts.Events;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class SessionProcessBridge : IHostedService
{
    private readonly IMessageRouter messageRouter;
    private readonly IIpcSerializer ipcSerializer;
    private readonly ISessionProcessTransport processTransport;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;
    private CancellationTokenSource cts = null!;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="SessionMonitor"/>
    /// </summary>
    public SessionProcessBridge(IMessageRouter messageRouter,
        IIpcSerializer ipcSerializer,
        ISessionProcessTransport processTransport,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.messageRouter = messageRouter;
        this.ipcSerializer = ipcSerializer;
        this.processTransport = processTransport;
        this.identityService = identityService;
        this.logger = logger;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        processTransport.MessageReceived += OnTransportMessage;
        return Task.CompletedTask;
    }

    private async void OnTransportMessage(object? sender, ProcessTransportMessageEventArgs e)
    {
        var envelope = JsonSerializer.Deserialize<IpcMessageEnvelope>(e.Data);
        if (envelope == null)
        {
            return;
        }

        var message = ipcSerializer.Deserialize(envelope.Body);
        await HandleIpcMessage(message, e.RoomId, e.Role, envelope.RecipientId);
    }

    private async Task HandleIpcMessage(IpcMessage message, Guid roomId, SessionProcessRole role, Guid? recipientId)
    {
        switch (message)
        {
            case InSessionMessage inSessionMessage:
                await messageRouter.RouteAsync(new SessionSpecificMessage()
                {
                    SubRoomId = inSessionMessage.SubRoomId,
                    SessionProcessRole = role,
                    Body = Encoding.UTF8.GetBytes(inSessionMessage.Body),
                    RecipientId = recipientId,
                }, roomId, identityService.SelfParticipant.Id, cts.Token);
                break;

            case ServerShutdownMessage serverShutdownMessage:
                await messageRouter.RouteAsync(new SessionServerShutdownMessage()
                {
                    SubRoomId = serverShutdownMessage.SubRoomId,
                    Reason = serverShutdownMessage.Reason,
                }, roomId, identityService.SelfParticipant.Id, cts.Token);
                break;

            default:
                return;
        }
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
