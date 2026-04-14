using System.IO.Pipes;
using System.Collections.Concurrent;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Discovery.Transport.NamedPipes.Services;

namespace MIN.Discovery.Transport.NamedPipes.Server;

/// <summary>
/// Сервер Named pipes для обнаружения комнаты в сети
/// </summary>
internal sealed class NamedPipeDiscoveryServer : IAsyncDisposable
{
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, NamedPipeServerStream> connections = new();
    private readonly string pipeName;

    private CancellationTokenSource? cts;
    private bool isListening;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeDiscoveryServer"/>
    /// </summary>
    public NamedPipeDiscoveryServer(ILocalNetworkComputerProvider localNetworkComputerProvider, ILoggerProvider logger)
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
        Guid? targetId;
        NamedPipeServerStream? targetPipe;

        if (connectionId.HasValue && connections.TryGetValue(connectionId.Value, out var p) && p.IsConnected)
        {
            targetPipe = p;
            targetId = connectionId.Value;
        }
        else
        {
            logger.Log("Сервер обнаружение не был подключен при передаче данных", LogLevel.Error);
            return;
        }

        await targetPipe.WriteAsync(responseData.AsMemory(), cancellationToken);
        await targetPipe.FlushAsync(cancellationToken);
        await targetPipe.DisposeAsync();

        if (targetId.HasValue)
        {
            connections.TryRemove(targetId.Value, out _);
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
        var buffer = new byte[4096];
        if (!connections.TryGetValue(connectionId, out var currentPipe) || currentPipe == null)
        {
            return;
        }
        var bytesRead = await currentPipe.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        if (bytesRead > 0)
        {
            var data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead);
            MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(data, connectionId));
        }
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await StopListeningAsync();
}
