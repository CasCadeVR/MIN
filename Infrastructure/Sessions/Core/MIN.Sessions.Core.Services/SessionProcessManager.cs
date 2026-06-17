using System.Diagnostics;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <inheritdoc cref="ISessionProcessManager"/>
public class SessionProcessManager : ISessionProcessManager
{
    private const int ProcessWaitingTimeOutMs = 5000;
    private readonly Dictionary<ProcessContext, Process> pendingProcesses = [];
    private readonly Dictionary<ProcessContext, Process> runningProcesses = [];
    private readonly IMessageRouter messageRouter;
    private readonly ISessionProcessBridge processBridge;
    private readonly ISessionProcessTransport processTransport;
    private readonly ISubRoomManager subRoomManager;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionProcessManager"/>
    /// </summary>
    public SessionProcessManager(IMessageRouter messageRouter,
        ISessionProcessBridge processBridge,
        ISessionProcessTransport processTransport,
        ISubRoomManager subRoomManager,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.messageRouter = messageRouter;
        this.processBridge = processBridge;
        this.processTransport = processTransport;
        this.subRoomManager = subRoomManager;
        this.identityService = identityService;
        this.logger = logger;
    }

    async Task<bool> ISessionProcessManager.StartAsync(string gameExePath, ProcessContext context, CancellationToken cancellationToken)
    {
        logger.Log($"Стартую {gameExePath} как {context.Role}");

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

        pendingProcesses[context] = startedProcess;

        await processTransport.WaitForConnectionAsync(context, ProcessWaitingTimeOutMs, cancellationToken);
        var readySuccess = await processBridge.WaitForReadyMessage(context, ProcessWaitingTimeOutMs, cancellationToken);
        pendingProcesses.Remove(context);
        if (readySuccess == false)
        {
            startedProcess.Kill();
            return false;
        }

        pendingProcesses.Remove(context);
        runningProcesses[context] = startedProcess;

        startedProcess.EnableRaisingEvents = true;
        startedProcess.Exited += async (_, _) =>
        {
            if (context.Role == SessionProcessRole.Server)
            {
                if (subRoomManager.GetParticipantCount(context.RoomId, context.SubRoomId) == 0)
                {
                    return;
                }

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

    Task ISessionProcessManager.StopAsync(ProcessContext context)
    {
        if (runningProcesses.TryGetValue(context, out var process))
        {
            process.Kill();
        }
        return Task.CompletedTask;
    }

    Task ISessionProcessManager.StopForRoomAsync(Guid roomId)
    {
        var roomPendingProcesses = pendingProcesses.Keys.Where(x => x.RoomId == roomId);
        foreach (var context in roomPendingProcesses)
        {
            pendingProcesses[context].EnableRaisingEvents = false;
            pendingProcesses[context].Kill();
        }

        var roomRunningProcesses = runningProcesses.Keys.Where(x => x.RoomId == roomId);
        foreach (var context in roomRunningProcesses)
        {
            runningProcesses[context].EnableRaisingEvents = false;
            runningProcesses[context].Kill();
        }
        return Task.CompletedTask;
    }

    Task ISessionProcessManager.StopAllAsync()
    {
        foreach (var process in pendingProcesses.Values)
        {
            process.EnableRaisingEvents = false;
            process.Kill();
        }
        foreach (var process in runningProcesses.Values)
        {
            process.EnableRaisingEvents = false;
            process.Kill();
        }
        return Task.CompletedTask;
    }
}
