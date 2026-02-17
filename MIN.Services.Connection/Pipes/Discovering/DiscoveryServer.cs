using System.IO.Pipes;
using System.Diagnostics;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Contracts.Models;
using MIN.Services.Connection.Contracts.Interfaces.Discovering;

namespace MIN.Services.Connection.Pipes.Discovering
{
    /// <summary>
    /// Сервер для обнаружения
    /// </summary>
    public class DiscoveryServer : IDiscoveryServer, IAsyncDisposable
    {
        private readonly string pcName;
        private readonly Room room;
        private readonly IPipeMessageSerializer serializer;
        private CancellationTokenSource? cancellationTokenSource;
        private bool isRunning = false;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DiscoveryServer"/>
        /// </summary>
        public DiscoveryServer(string pcName, Room room, IPipeMessageSerializer serializer)
        {
            this.pcName = pcName;
            this.room = room;
            this.serializer = serializer;
        }

        async Task IDiscoveryServer.StartAsync(CancellationToken cancellationToken = default)
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
                    using var pipe = new NamedPipeServerStream(
                        DiscoveryPipeNameProvider.GetDiscoveryPipeName(pcName),
                        PipeDirection.InOut,
                        1,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous | PipeOptions.WriteThrough
                    );

                    await pipe!.WaitForConnectionAsync(ct);

                    // Отправляем информацию о комнате
                    var roomInfo = new DiscoveredRoom
                    {
                        Room = room.GetSerializableCopy(),
                        DiscoveredAt = DateTime.UtcNow,
                    };

                    await serializer.WriteMessageAsync(pipe!, roomInfo, ct);
                    //pipe.Disconnect();
                }
                catch (OperationCanceledException)
                {
                    break; // Ожидаемое завершение
                }
                catch (IOException ex) when (ex.Message.Contains("pipe is being closed"))
                {
                    // Игнорируем ошибки при закрытии канала
                    continue;
                }
                catch (Exception ex)
                {
                    // Логируем неожиданные ошибки, но продолжаем работу
                    Debug.WriteLine($"Discovery error: {ex.Message}");
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
            cancellationTokenSource?.Cancel();
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }
}
