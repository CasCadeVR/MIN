using System.Text;
using System.Text.Json;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.Contracts.Models;
using MIN.Sessions.Core.Messaging.Ipc;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Serialization.Contracts;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;
using MIN.Sessions.Core.Transport.Contracts.Events;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class SessionProcessBridge : ISessionProcessBridge
{
    private readonly Dictionary<ProcessContext, TaskCompletionSource> pendingProcesses = [];
    private readonly IMessageRouter messageRouter;
    private readonly IIpcSerializer ipcSerializer;
    private readonly ISessionProcessTransport processTransport;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;
    private CancellationTokenSource cts = null!;

    /// <summary>
    /// Инициализирует новый экзепмляр <see cref="SessionProcessBridge"/>
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

    Task ISessionProcessBridge.StartListeningAsync(CancellationToken cancellationToken)
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
        await HandleIpcMessage(message, e.Context, envelope.RecipientId);
    }

    private async Task HandleIpcMessage(IpcMessage message, ProcessContext context, Guid? recipientId)
    {
        switch (message)
        {
            case InSessionMessage inSessionMessage:
                await messageRouter.RouteAsync(new SessionSpecificMessage()
                {
                    SubRoomId = context.SubRoomId,
                    SessionProcessRole = context.Role,
                    Body = Encoding.UTF8.GetBytes(inSessionMessage.Body),
                    RecipientId = recipientId,
                }, context.RoomId, identityService.SelfParticipant.Id, cts.Token);
                break;

            case ServerShutdownMessage serverShutdownMessage:
                await messageRouter.RouteAsync(new SessionServerShutdownMessage()
                {
                    SubRoomId = context.SubRoomId,
                    Reason = serverShutdownMessage.Reason,
                }, context.RoomId, identityService.SelfParticipant.Id, cts.Token);
                break;

            case ReadyMessage:
                pendingProcesses[context].TrySetResult();
                break;

            default:
                return;
        }
    }

    async Task<bool> ISessionProcessBridge.WaitForReadyMessage(ProcessContext context, int timeOutMs, CancellationToken cancellationToken)
    {
        pendingProcesses[context] = new TaskCompletionSource();

        try
        {
            processTransport.MessageReceived += OnTransportMessage;
            await pendingProcesses[context].Task.WaitAsync(TimeSpan.FromMilliseconds(timeOutMs), cancellationToken);
        }
        catch
        {
            return false;
        }
        finally
        {
            processTransport.MessageReceived -= OnTransportMessage;
            pendingProcesses.Remove(context);
        }
        return true;
    }


    IEnumerable<ProcessContext> ISessionProcessBridge.GetConnections(Guid roomId, int subRoomId)
    {
        foreach (SessionProcessRole role in Enum.GetValues(typeof(SessionProcessRole)))
        {
            var context = new ProcessContext(roomId, subRoomId, role);
            if (processTransport.IsConnectionExists(context) && !pendingProcesses.ContainsKey(context))
            {
                yield return context;
            }
        }
    }

    async Task ISessionProcessBridge.SendIpcMessage(IpcMessage message, ProcessContext context, CancellationToken cancellationToken)
    {
        var data = ipcSerializer.Serialize(message);
        await SendData(data, context, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SendData(byte[] data, ProcessContext context, CancellationToken cancellationToken)
    {
        await processTransport.SendAsync(data, context, cancellationToken);
    }

    Task ISessionProcessBridge.StopListeningAsync(CancellationToken cancellationToken)
    {
        cts.Cancel();
        cts.Dispose();
        return Task.CompletedTask;
    }
}
