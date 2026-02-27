using System.IO.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Contracts.Models;
using MIN.Services.Connection.Contracts.Interfaces.Discovering;
using System.Security.Principal;
using System.Security.AccessControl;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models.Enums;

namespace MIN.Services.Connection.Pipes.Discovering
{
    /// <summary>
    /// Сервер для обнаружения
    /// </summary>
    public class DiscoveryServer : IDiscoveryServer, IAsyncDisposable
    {
        private readonly IPipeMessageSerializer serializer;
        private readonly ILoggerProvider logger;
        private readonly Participant hostParticipant;
        private readonly Room room;

        private CancellationTokenSource? cancellationTokenSource;
        private bool isRunning = false;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DiscoveryServer"/>
        /// </summary>
        public DiscoveryServer(Participant hostParticipant, Room room, IPipeMessageSerializer serializer, ILoggerProvider logger)
        {
            this.hostParticipant = hostParticipant;
            this.room = room;
            this.serializer = serializer;
            this.logger = logger;
        }

        async Task IDiscoveryServer.StartAsync(CancellationToken cancellationToken)
        {
            if (isRunning) await StopAsync();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            isRunning = true;
            _ = AcceptDiscoveryRequestsAsync(cancellationTokenSource.Token);
        }

        private async Task AcceptDiscoveryRequestsAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && isRunning)
            {
                try
                {
                    if (!OperatingSystem.IsWindows())
                    {
                        throw new PlatformNotSupportedException("Windows only");
                    }

                    var securityIdentifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                    var pipeSecurity = new PipeSecurity();
                    pipeSecurity.AddAccessRule(
                        new PipeAccessRule(securityIdentifier,
                            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
                            AccessControlType.Allow));

                    using var pipe = NamedPipeServerStreamAcl.Create(
                        DiscoveryPipeNameProvider.GetDiscoveryPipeName(hostParticipant.PCName),
                        PipeDirection.InOut,
                        1,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                        0, 0,
                        pipeSecurity);

                    await pipe!.WaitForConnectionAsync(ct);

                    var roomInfo = new DiscoveredRoom
                    {
                        RoomId = room.Id,
                        RoomName = room.Name,
                        HostId = room.HostParticipant.Id,
                        HostName = room.HostParticipant.Name,
                        HostPCName = room.HostParticipant.PCName,
                        MaximumParticipants = room.MaximumParticipants,
                        CurrentParticipants = room.CurrentParticipants.Count,
                    };

                    await serializer.WriteMessageAsync(pipe!, roomInfo, hostParticipant.Id, ct);
                }
                catch (OperationCanceledException)
                {
                    logger.Log("Сервер обнаружения был остановлен", LogLevel.Information);
                    break;
                }
                catch (IOException ex) when (ex.Message.Contains("pipe is being closed"))
                {
                    // Игнорируем ошибки при закрытии канала
                    continue;
                }
                catch (Exception ex)
                {
                    logger.Log($"Сервер обнаружения поймал ошибку: {ex.Message}", LogLevel.Error);
                    continue;
                }
            }
        }

        async Task IDiscoveryServer.StopAsync()
        {
            await StopAsync();
        }

        /// <inheritdoc cref="IDiscoveryServer.StopAsync"/>
        public async Task StopAsync()
        {
            await cancellationTokenSource!.CancelAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }
}
