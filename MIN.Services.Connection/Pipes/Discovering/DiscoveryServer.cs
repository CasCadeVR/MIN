using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Services.Connection.Contracts.Interfaces.Discovering;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Enums;

namespace MIN.Services.Connection.Pipes.Discovering
{
    /// <summary>
    /// Сервер для обнаружения
    /// </summary>
    /// <remarks>
    /// Инициализирует новый экземпляр <see cref="DiscoveryServer"/>
    /// </remarks>
    public class DiscoveryServer(Participant hostParticipant, Room room, IPipeMessageSerializer serializer, ILoggerProvider logger) : IDiscoveryServer, IAsyncDisposable
    {
        private readonly IPipeMessageSerializer serializer = serializer;
        private readonly ILoggerProvider logger = logger;
        private readonly Participant hostParticipant = hostParticipant;
        private readonly Room room = room;
        private readonly SemaphoreSlim requestSemaphore = new(1, 1);

        private CancellationTokenSource? cancellationTokenSource;
        private PipeSecurity? pipeSecurity;

        async Task IDiscoveryServer.StartAsync(CancellationToken cancellationToken)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("Windows only");
            }

            pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));

            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = AcceptDiscoveryRequestsAsync(cancellationTokenSource.Token);
        }

        private async Task AcceptDiscoveryRequestsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await requestSemaphore.WaitAsync(cancellationToken);

                    await WaitForClientAndAcceptIt(cancellationToken);
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
                finally
                {
                    requestSemaphore.Release();
                }
            }
        }

        private async Task WaitForClientAndAcceptIt(CancellationToken cancellationToken)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("Windows only");
            }

            using var pipe = NamedPipeServerStreamAcl.Create(
                DiscoveryPipeNameProvider.GetDiscoveryPipeName(hostParticipant.PCName),
                PipeDirection.InOut,
                room.MaximumParticipants,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous,
                0, 0,
                pipeSecurity);

            await pipe.WaitForConnectionAsync(cancellationToken);

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

            await serializer.WriteMessageAsync(pipe, roomInfo, Guid.Empty, cancellationToken);
        }

        async Task IDiscoveryServer.StopAsync()
        {
            await StopAsync();
        }

        /// <inheritdoc cref="IDiscoveryServer.StopAsync"/>
        public async Task StopAsync()
        {
            requestSemaphore.Dispose();
            await cancellationTokenSource!.CancelAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }
}
