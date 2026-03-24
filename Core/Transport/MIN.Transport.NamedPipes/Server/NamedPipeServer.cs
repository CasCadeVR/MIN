using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Constants;
using MIN.Transport.NamedPipes.Models;

namespace MIN.Transport.NamedPipes.Server;

/// <summary>
/// Сервер Named Pipe для комнаты
/// </summary>
internal sealed class NamedPipeServer : IAsyncDisposable
{
    private readonly NamedPipeEndpoint endpoint;
    private readonly SemaphoreSlim connectionSlots;
    private readonly ConcurrentDictionary<Guid, NamedPipeConnection> connections = new();

    private readonly ILoggerProvider logger;

    private CancellationTokenSource? cts;
    private bool isRunning;

    /// <summary>
    /// Инициализирует новый экзмепляр <see cref="NamedPipeServer"/>
    /// </summary>
    public NamedPipeServer(
        NamedPipeEndpoint endpoint,
        int maxParticipants,
        ILoggerProvider logger)
    {
        this.endpoint = endpoint;
        this.logger = logger;

        connectionSlots = new SemaphoreSlim(maxParticipants, maxParticipants);
    }

    /// <summary>
    /// Событие, возникающее при получении сообщения от участника
    /// </summary>
    public event EventHandler<(Guid ConnectionId, byte[] Data)>? RawMessageReceived;

    /// <summary>
    /// Соеднинение сервера и клиента разорвалось
    /// </summary>
    public event EventHandler<(Guid ConnectionId, string? Reason)>? ConnectionDisconnected;

    /// <summary>
    /// Запустить сервер
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (isRunning)
        {
            return;
        }

        isRunning = true;
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await Task.Run(() => AcceptClientsAsync(cts.Token), cts.Token);
    }

    /// <summary>
    /// Останавить сервер и закрывает все соединения
    /// </summary>
    public async Task StopAsync()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        cts?.Cancel();

        foreach (var connection in connections.Values)
        {
            await connection.DisposeAsync();
        }

        connections.Clear();
        connectionSlots.Dispose();
    }

    /// <summary>
    /// Отправить сообщение конкретному соединению
    /// </summary>
    public async Task SendToConnectionAsync(byte[] data, Guid connectionId, CancellationToken cancellationToken = default)
    {
        if (connections.TryGetValue(connectionId, out var connection))
        {
            await connection.SendAsync(data, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"Connection {connectionId} not found");
        }
    }

    /// <summary>
    /// Разослать сообщение всем участникам (кроме исключённого)
    /// </summary>
    public async Task BroadcastAsync(byte[] data, IEnumerable<Guid>? excludeConnections = null, CancellationToken cancellationToken = default)
    {
        var tasks = connections.Values
           .Where(c => excludeConnections == null || !excludeConnections.Contains(c.Id))
           .Select(c => c.SendAsync(data, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("К сожалению, пока только на Windows");
        }

        var pipeSecurity = new PipeSecurity();
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
            AccessControlType.Allow));

        while (!cancellationToken.IsCancellationRequested && isRunning)
        {
            try
            {
                await connectionSlots.WaitAsync(cancellationToken);

                var pipe = NamedPipeServerStreamAcl.Create(
                    endpoint.PipeName,
                    PipeDirection.InOut,
                    TransportConstants.TheoraticallyPossibleMaximumRoomSize,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                    0, 0,
                    pipeSecurity);

                await pipe.WaitForConnectionAsync(cancellationToken);

                // Client connected here

                var connection = new NamedPipeConnection(pipe, endpoint);
                connections[connection.Id] = connection;

                connection.RawMessageReceived += (_, data) =>
                {
                    RawMessageReceived?.Invoke(this, (connection.Id, data));
                };
                connection.Disconnected += (_, reason) =>
                {
                    OnConnectionDisconnected(connection, reason);
                };

                await connection.StartReadingAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время принятия клиента: {ex.Message}");
                connectionSlots.Release();
            }
        }
    }

    private void OnConnectionDisconnected(NamedPipeConnection connection, string? reason)
    {
        connections.TryRemove(connection.Id, out _);
        connectionSlots.Release();
        ConnectionDisconnected?.Invoke(this, (connection.Id, reason ?? string.Empty));
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public async ValueTask DisposeAsync() => await StopAsync();
}
