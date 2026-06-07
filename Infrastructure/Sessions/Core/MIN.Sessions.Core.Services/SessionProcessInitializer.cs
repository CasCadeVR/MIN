using System.Diagnostics;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging.OutOfSubRoom;
using MIN.Sessions.Core.Services.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Enums;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <inheritdoc cref="ISessionProcessInitializer"/>
public class SessionProcessInitializer : ISessionProcessInitializer
{
    private readonly Dictionary<string, Process> runningProcesses = [];
    private readonly IMessageRouter messageRouter;
    private readonly ISessionProcessTransport processTransport;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionProcessInitializer"/>
    /// </summary>
    public SessionProcessInitializer(IMessageRouter messageRouter,
        ISessionProcessTransport processTransport,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.messageRouter = messageRouter;
        this.processTransport = processTransport;
        this.identityService = identityService;
        this.logger = logger;
    }

    async Task<bool> ISessionProcessInitializer.StartAsync(Guid roomId, int subRoomId, string gameExePath, SessionProcessRole sessionProcessRole, CancellationToken cancellationToken)
    {
        logger.Log($"Стартую {gameExePath} как {SessionProcessRole.Server}");

        //var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, gameExePath);
        var fullPath = gameExePath;
        if (!Path.Exists(fullPath))
        {
            return false;
        }

        await processTransport.StartAsync(roomId, cancellationToken);
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

        var connection = await processTransport.WaitForConnectionAsync(roomId, subRoomId, sessionProcessRole, cancellationToken);

        startedProcess.EnableRaisingEvents = true;
        startedProcess.Exited += async (_, _) =>
        {
            if (sessionProcessRole == SessionProcessRole.Server)
            {
                await messageRouter.RouteAsync(new SessionServerShutdownMessage()
                {
                    SubRoomId = subRoomId,
                    Reason = "Сервер сессии был закрыт хостом"
                }, roomId, identityService.SelfParticipant.Id, cancellationToken);
            }
            else
            {
                await messageRouter.RouteAsync(new SessionLeaveMessage()
                {
                    SubRoomId = subRoomId,
                }, roomId, identityService.SelfParticipant.Id, cancellationToken);
            }
        };

        var key = string.Join('/', roomId, subRoomId, sessionProcessRole);
        runningProcesses[key] = startedProcess;

        return true;
    }

    Task ISessionProcessInitializer.StopAsync(Guid roomId, int subRoomId, SessionProcessRole sessionProcessRole)
    {
        var key = string.Join('/', roomId, subRoomId, sessionProcessRole);
        if (runningProcesses.TryGetValue(key, out var process))
        {
            process.Kill();
        }
        return Task.CompletedTask;
    }
}
