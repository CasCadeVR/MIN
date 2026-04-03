using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Discovery.Transport.NamedPipes.Server;

/// <summary>
/// Сервер Named pipes для обнаружения комнаты в сети
/// </summary>
internal sealed class NamedPipeDiscoveryServer : IAsyncDisposable
{
    private readonly ILoggerProvider logger;
    private CancellationTokenSource? cts;
    private NamedPipeServerStream? pipe;
    private string pipeName;
    private bool isListening;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryServer"/>
    /// </summary>
    public NamedPipeDiscoveryServer(string pipeName, ILoggerProvider logger)
    {
        this.pipeName = pipeName;
        this.logger = logger;
    }

    /// <summary>
    /// Получен запрос на обнаружение
    /// </summary>
    public event EventHandler<DiscoveryRawMessageReceivedEventArgs>? MessageReceived;

    public async Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        if (isListening)
        {
            return;
        }

        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        isListening = true;
        _ = ListenAsync(cts.Token);
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("К сожалению, пока только на Windows");
        }

        var security = new PipeSecurity();
        security.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
            AccessControlType.Allow));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                pipe = NamedPipeServerStreamAcl.Create(
                    pipeName,
                    PipeDirection.InOut,
                    2,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous,
                    0, 0,
                    security);

                await pipe.WaitForConnectionAsync(cancellationToken);
                await AcceptRequest(cancellationToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                logger.Log($"Ошибка в ответе на обнаружение: {ex.Message}", LogLevel.Error);
            }
        }
    }

    /// <summary>
    /// Ответить клиенту на запрос
    /// </summary>
    public async Task ResponseWithData(byte[] responseData, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        if (pipe == null)
        {
            return;
        }

        await pipe.WriteAsync(responseData.AsMemory(), cancellationToken);
        await pipe.FlushAsync(cancellationToken);
        await pipe.DisposeAsync();
    }

    public async Task StopListeningAsync()
    {
        if (!isListening)
        {
            return;
        }

        cts?.Cancel();
        pipe?.Dispose();

        isListening = false;
        await Task.CompletedTask;
    }

    private async Task AcceptRequest(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];
        var bytesRead = await pipe!.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        if (bytesRead > 0)
        {
            var data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead);
            MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(data));
        }
    }

    public async ValueTask DisposeAsync() => await StopListeningAsync();
}
