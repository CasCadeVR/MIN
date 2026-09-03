using System.Diagnostics;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Services.Contracts.Models;
using MIN.Sessions.Core.Transport.Contracts.Enums;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Models;

namespace MIN.Sessions.Core.Services;

/// <inheritdoc cref="ISessionProcessManager"/>
public class SessionProcessManager : ISessionProcessManager
{
    private const int ProcessWaitingTimeOutMs = 30_000;

    private readonly Dictionary<ProcessContext, Process> pendingProcesses = [];
    private readonly Dictionary<ProcessContext, Process> runningProcesses = [];
    private readonly Dictionary<ProcessContext, ISessionProcessTransport> transports = [];
    private readonly IMessageRouter messageRouter;
    private readonly ISessionProcessBridge processBridge;
    private readonly ISessionTransportFactory transportFactory;
    private readonly IRoomConnectionRegistry registry;
    private readonly ISubRoomManager subRoomManager;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    private EventHandler? currentExitHandler;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionProcessManager"/>
    /// </summary>
    public SessionProcessManager(IMessageRouter messageRouter,
        ISessionProcessBridge processBridge,
        ISessionTransportFactory transportFactory,
        IRoomConnectionRegistry registry,
        ISubRoomManager subRoomManager,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.messageRouter = messageRouter;
        this.processBridge = processBridge;
        this.transportFactory = transportFactory;
        this.registry = registry;
        this.subRoomManager = subRoomManager;
        this.identityService = identityService;
        this.logger = logger;
    }

    async Task<bool> ISessionProcessManager.StartAsync(Session session, ProcessContext context, CancellationToken cancellationToken)
    {
        var fullPath = context.Role == SessionProcessRole.Client
            ? session.GetClientPath()
            : session.GetServerPath();

        logger.Log($"Стартую {session.Name} как {context.Role}");

        if (!Path.Exists(fullPath))
        {
            return false;
        }

        var processTransport = transportFactory.Create();
        transports[context] = processTransport;
        processBridge.RegisterTransport(context, processTransport);

        await processTransport.StartAsync(context.RoomId, cancellationToken);
        var connectionString = processTransport.GetConnectionString();

        var psi = new ProcessStartInfo
        {
            FileName = fullPath,
            UseShellExecute = false,
        };
        psi.ArgumentList.Add(connectionString);

        if (!OperatingSystem.IsWindows())
        {
            var mode = File.GetUnixFileMode(fullPath);
            if ((mode & UnixFileMode.UserExecute) == 0)
            {
                psi.FileName = "dotnet";
                psi.ArgumentList.Insert(0, fullPath);
            }
        }

        var startedProcess = Process.Start(psi);

        if (startedProcess == null || startedProcess.HasExited)
        {
            processBridge.UnregisterTransport(context);
            return false;
        }

        pendingProcesses[context] = startedProcess;

        var connectCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await processTransport.WaitForConnectionAsync(context, ProcessWaitingTimeOutMs, connectCts.Token);
        var readySuccess = await processBridge.WaitForReadyMessage(context, ProcessWaitingTimeOutMs, connectCts.Token);
        pendingProcesses.Remove(context);
        if (readySuccess == false)
        {
            processBridge.UnregisterTransport(context);
            startedProcess.Kill();
            return false;
        }

        pendingProcesses.Remove(context);
        runningProcesses[context] = startedProcess;

        startedProcess.EnableRaisingEvents = true;
        currentExitHandler = async (_, _) => await AnnounceExit(session, context, cancellationToken);
        startedProcess.Exited += currentExitHandler;

        return true;
    }

    private async Task AnnounceExit(Session session, ProcessContext context, CancellationToken cancellationToken)
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
                Reason = $"Сервер сессии {session.Name} был закрыт хостом"
            }, context.RoomId, identityService.SelfParticipant.Id, cancellationToken);
        }
        else
        {
            await messageRouter.RouteAsync(new SessionLeaveMessage()
            {
                SubRoomId = context.SubRoomId,
            }, context.RoomId, identityService.SelfParticipant.Id, cancellationToken);
        }
        runningProcesses.Remove(context);
    }

    bool ISessionProcessManager.SessionClientAppExists(Session session)
        => Path.Exists(session.ClientExecutableFileName);

    async Task ISessionProcessManager.StopAsync(ProcessContext context)
    {
        if (runningProcesses.TryGetValue(context, out var process))
        {
            await StopProcessWithTimeOut(context, process, clearAnnounce: false);
        }
        runningProcesses.Remove(context);
    }

    async Task ISessionProcessManager.StopForRoomAsync(Guid roomId)
    {
        var roomPendingProcesses = pendingProcesses.Keys.Where(x => x.RoomId == roomId).ToList();
        foreach (var context in roomPendingProcesses)
        {
            await StopProcessWithTimeOut(context, pendingProcesses[context]);
            pendingProcesses.Remove(context);
        }

        var roomRunningProcesses = runningProcesses.Keys.Where(x => x.RoomId == roomId).ToList();
        foreach (var context in roomRunningProcesses)
        {
            await StopProcessWithTimeOut(context, runningProcesses[context]);
            runningProcesses.Remove(context);
        }
    }

    async Task ISessionProcessManager.StopAllAsync()
    {
        foreach (var process in pendingProcesses)
        {
            await StopProcessWithTimeOut(process.Key, process.Value);
        }
        pendingProcesses.Clear();
        foreach (var process in runningProcesses)
        {
            await StopProcessWithTimeOut(process.Key, process.Value);
        }
        runningProcesses.Clear();
    }

    private async Task StopProcessWithTimeOut(ProcessContext context, Process process, bool clearAnnounce = true)
    {
        if (currentExitHandler != null && clearAnnounce)
        {
            process.Exited -= currentExitHandler;
        }

        await processBridge.SendCloseMessage(context, CancellationToken.None);

        var exited = await Task.WhenAny(
            process.WaitForExitAsync(CancellationToken.None),
            Task.Delay(ProcessWaitingTimeOutMs)
        ) == process.WaitForExitAsync(CancellationToken.None);

        if (exited)
        {
            transportFactory.Destroy(transports[context]);
            processBridge.UnregisterTransport(context);
        }
        else
        {
            process.Kill();
            await process.WaitForExitAsync(CancellationToken.None);
            transportFactory.Destroy(transports[context]);
            processBridge.UnregisterTransport(context);
        }
    }
}
