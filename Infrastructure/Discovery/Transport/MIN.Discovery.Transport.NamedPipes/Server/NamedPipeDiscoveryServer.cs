using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Discovery.Services.Contracts.Models.Constants;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Discovery.Transport.NamedPipes.Services;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Discovery.Transport.NamedPipes.Server;

/// <summary>
/// Сервер Named pipes для обнаружения комнаты в сети
/// </summary>
internal sealed class NamedPipeDiscoveryServer : IAsyncDisposable
{
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, NamedPipeServerStream> connections = new();
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<byte[]>> responseWaiters = new();
    private readonly string pipeName;

    private CancellationTokenSource? cts;
    private bool isListening;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeDiscoveryServer"/>
    /// </summary>
    public NamedPipeDiscoveryServer(ILocalNetworkComputerProvider localNetworkComputerProvider,
        ILoggerProvider logger)
    {
        this.logger = logger;
        pipeName = DiscoveryPipeNameProvider.GetDiscoveryPipeName(localNetworkComputerProvider.GetLocalMachineName());
    }

    /// <summary>
    /// Получен запрос на обнаружение
    /// </summary>
    public event EventHandler<DiscoveryRawMessageReceivedEventArgs>? MessageReceived;

    public Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        if (isListening)
        {
            return Task.CompletedTask;
        }

        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        isListening = true;
        _ = ListenAsync(cts.Token);
        return Task.CompletedTask;
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
                var newPipe = NamedPipeServerStreamAcl.Create(
                    pipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous,
                    0, 0,
                    security);

                var connectionId = Guid.NewGuid();
                connections.TryAdd(connectionId, newPipe);

                await newPipe.WaitForConnectionAsync(cancellationToken);
                await AcceptRequest(connectionId, cancellationToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                var message = $"Ошибка в ответе на обнаружение: {ex.Message}";
                logger.Log(message, LogLevel.Error);
                throw new InvalidOperationException(message);
            }
        }
    }

    /// <summary>
    /// Ответить клиенту на запрос
    /// </summary>
    public async Task ResponseWithData(byte[] responseData, Guid? connectionId, CancellationToken cancellationToken)
    {
        if (connectionId.HasValue && connections.TryGetValue(connectionId.Value, out var pipe) && pipe.IsConnected)
        {
            try
            {
                await pipe.WriteAsync(responseData.AsMemory(), cancellationToken);
                await pipe.FlushAsync(cancellationToken);

                if (responseWaiters.TryGetValue(connectionId.Value, out var tcs))
                {
                    tcs.SetResult(responseData);
                }
            }
            catch
            {
                if (responseWaiters.TryGetValue(connectionId.Value, out var tcs))
                {
                    tcs.SetException(new TimeoutException());
                }
            }
        }
    }

    public async Task StopListeningAsync()
    {
        if (!isListening)
        {
            return;
        }

        cts?.Cancel();
        foreach (var connection in connections)
        {
            connection.Value?.Dispose();
        }
        connections.Clear();

        isListening = false;
        await Task.CompletedTask;
    }

    private async Task AcceptRequest(Guid connectionId, CancellationToken cancellationToken)
    {
        var buffer = new byte[DiscoveryConstants.DiscoveryBufferSize];
        var pipe = connections[connectionId];
        var bytesRead = await pipe.ReadAsync(buffer.AsMemory(), cancellationToken);

        if (bytesRead > 0)
        {
            var data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead);

            var tcs = new TaskCompletionSource<byte[]>();
            responseWaiters[connectionId] = tcs;

            MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(data, pipeName, connectionId));

            // Wait for response but with timeout
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(5));

            try
            {
                await tcs.Task.WaitAsync(timeoutCts.Token);
            }
            catch (TimeoutException) { }
        }

        // Cleanup
        responseWaiters.TryRemove(connectionId, out _);
        await pipe.DisposeAsync();
        connections.TryRemove(connectionId, out _);
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await StopListeningAsync();
}
