using System.Diagnostics;
using System.Text.Json;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.Contracts.Enums;
using MIN.Sessions.Core.Messaging.Contracts.Models;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Serialization.Contracts;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;
using MIN.Sessions.Core.Transport.Contracts.Events;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <inheritdoc cref="ISessionProcessInitializer"/>
public class SessionProcessInitializer : ISessionProcessInitializer
{
    private const int ProcessWaitingTimeOutMs = 10_000;
    private readonly Dictionary<ProcessContext, Process> runningProcesses = [];
    private readonly Dictionary<ProcessContext, TaskCompletionSource> pendingProcesses = [];
    private readonly IMessageRouter messageRouter;
    private readonly IIpcSerializer ipcSerializer;
    private readonly ISessionProcessTransport processTransport;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionProcessInitializer"/>
    /// </summary>
    public SessionProcessInitializer(IMessageRouter messageRouter,
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

    async Task<bool> ISessionProcessInitializer.StartAsync(string gameExePath, ProcessContext context, CancellationToken cancellationToken)
    {
        logger.Log($"Стартую {gameExePath} как {SessionProcessRole.Server}");

        //var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, gameExePath);
        var fullPath = gameExePath;
        if (!Path.Exists(fullPath))
        {
            return false;
        }

        await processTransport.StartAsync(context.RoomId, cancellationToken);
        var connectionString = processTransport.GetConnectionString();

        var startedProcess = Process.Start(new ProcessStartInfo
        {
            FileName = fullPath,
            ArgumentList = { connectionString },
            UseShellExecute = false,
        });

        if (startedProcess == null || startedProcess.HasExited)
        {
            return false;
        }

        runningProcesses[context] = startedProcess;

        await processTransport.WaitForConnectionAsync(context, ProcessWaitingTimeOutMs, cancellationToken);
        var readySuccess = await WaitForReadyMessage(context, cancellationToken);
        if (readySuccess == false)
        {
            return false;
        }

        startedProcess.EnableRaisingEvents = true;
        startedProcess.Exited += async (_, _) =>
        {
            if (context.Role == SessionProcessRole.Server)
            {
                await messageRouter.RouteAsync(new SessionServerShutdownMessage()
                {
                    SubRoomId = context.SubRoomId,
                    Reason = "Сервер сессии был закрыт хостом"
                }, context.RoomId, identityService.SelfParticipant.Id, cancellationToken);
            }
            else
            {
                await messageRouter.RouteAsync(new SessionLeaveMessage()
                {
                    SubRoomId = context.SubRoomId,
                }, context.RoomId, identityService.SelfParticipant.Id, cancellationToken);
            }
        };

        return true;
    }

    private async void OnTransportMessage(object? sender, ProcessTransportMessageEventArgs e)
    {
        var envelope = JsonSerializer.Deserialize<IpcMessageEnvelope>(e.Data);
        if (envelope == null)
        {
            return;
        }

        var message = ipcSerializer.Deserialize(envelope.Body);
        if (message.Type == IpcMessageType.Ready)
        {
            pendingProcesses[e.Context].TrySetResult();
        }
    }

    private async Task<bool> WaitForReadyMessage(ProcessContext context, CancellationToken cancellationToken)
    {
        pendingProcesses[context] = new TaskCompletionSource();

        try
        {
            processTransport.MessageReceived += OnTransportMessage;
            await pendingProcesses[context].Task.WaitAsync(TimeSpan.FromMilliseconds(ProcessWaitingTimeOutMs), cancellationToken);
        }
        catch
        {
            return false;
        }
        finally
        {
            processTransport.MessageReceived -= OnTransportMessage;
            pendingProcesses[context] = null!;
        }
        return true;
    }

    Task ISessionProcessInitializer.StopAsync(ProcessContext context)
    {
        if (runningProcesses.TryGetValue(context, out var process))
        {
            process.Kill();
        }
        return Task.CompletedTask;
    }

    Task ISessionProcessInitializer.StopAll()
    {
        foreach (var process in runningProcesses.Values)
        {
            process.Kill();
        }
        return Task.CompletedTask;
    }
}
