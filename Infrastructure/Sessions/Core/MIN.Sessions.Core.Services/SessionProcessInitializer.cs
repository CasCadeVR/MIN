using System.Diagnostics;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging;
using MIN.Sessions.Core.Services.Contracts.Enums;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Core.Services;

/// <inheritdoc cref="ISessionProcessInitializer"/>
public class SessionProcessInitializer : ISessionProcessInitializer
{
    private readonly Dictionary<string, Process> runningProcesses = [];
    private readonly IMessageRouter messageRouter;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="SessionProcessInitializer"/>
    /// </summary>
    public SessionProcessInitializer(IMessageRouter messageRouter,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.messageRouter = messageRouter;
        this.identityService = identityService;
        this.logger = logger;
    }

    async Task<bool> ISessionProcessInitializer.StartAsync(Guid roomId, int subRoomId, string gameExePath, SessionProcessRole sessionProcessRole, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, gameExePath);
        if (!Path.Exists(fullPath))
        {
            return false;
        }

        var startedProcess = Process.Start(fullPath);
        if (startedProcess == null)
        {
            return false;
        }

        startedProcess.EnableRaisingEvents = true;
        startedProcess.Exited += async (_, _) =>
        {
            if (sessionProcessRole == SessionProcessRole.Client)
            {
                await messageRouter.RouteAsync(new SessionLeaveMessage()
                {
                    SubRoomId = subRoomId,
                }, roomId, identityService.SelfParticipant.Id, cancellationToken);
            }
        };

        var roleName = sessionProcessRole == SessionProcessRole.Server ? "Сервер" : "Клиент";
        logger.Log($"Стартую {gameExePath} как {roleName}");

        var key = string.Join('/', roomId, subRoomId, sessionProcessRole);
        runningProcesses[key] = startedProcess;

        return true;
    }

    Task ISessionProcessInitializer.StopAsync(Guid roomId, int subRoomId)
    {
        var key = string.Join('/', roomId, subRoomId);
        if (runningProcesses.TryGetValue(key, out var process))
        {
            process.Kill();
        }
        return Task.CompletedTask;
    }
}
