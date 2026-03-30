using System.Collections.Concurrent;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Constants;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Core.Transport.NamedPipes.Server;
using MIN.Core.Transport.NamedPipes.Client;

namespace MIN.Core.Transport.NamedPipes;

/// <summary>
/// Реализация транспорта на основе Named Pipes
/// </summary>
public sealed class NamedPipeTransport : ITransport
{
    private readonly ConcurrentDictionary<Guid, NamedPipeServer> servers = new();
    private readonly ConcurrentDictionary<Guid, NamedPipeClient> clients = new();
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeTransport"/>
    /// </summary>
    public NamedPipeTransport(ILoggerProvider logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc />
    public event EventHandler<RawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    async Task ITransport.StartHostingAsync(Guid roomId, IEndpoint endpoint, CancellationToken cancellationToken)
    {
        if (endpoint is not NamedPipeEndpoint namedPipeEndpoint)
        {
            throw new ArgumentException("Endpoint must be NamedPipeEndpoint", nameof(endpoint));
        }

        if (servers.ContainsKey(roomId))
        {
            return;
        }

        // TODO: максимальное количество участников нужно получить из конфигурации
        var server = new NamedPipeServer(namedPipeEndpoint, TransportConstants.TheoraticallyPossibleMaximumRoomSize, logger);

        server.RawMessageReceived += (_, args) =>
            RawMessageReceived?.Invoke(this, new RawMessageReceivedEventArgs(args.Data, roomId, args.ConnectionId));

        server.ConnectionDisconnected += (_, args) =>
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, args.ConnectionId, false, args.Reason));

        servers[roomId] = server;
        await server.StartAsync(cancellationToken);
        logger.Log($"Сервер стартанул комнату с id {roomId} на pipe {namedPipeEndpoint.PipeName}");
    }

    async Task ITransport.StopHostingAsync(Guid roomId)
    {
        if (servers.TryRemove(roomId, out var server))
        {
            await server.DisposeAsync();
            logger.Log($"Сервер был остановлен для комнаты {roomId}");
        }
    }

    async Task<Guid> ITransport.ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken)
    {
        if (endpoint is not NamedPipeEndpoint namedPipeEndpoint)
        {
            throw new ArgumentException("Endpoint должен быть NamedPipeEndpoint", nameof(endpoint));
        }

        if (clients.TryGetValue(roomId, out var existingClient) && existingClient.IsConnected)
        {
            return existingClient.ConnectionId;
        }

        var client = new NamedPipeClient(namedPipeEndpoint, logger);

        client.RawMessageReceived += (_, data) =>
            RawMessageReceived?.Invoke(this, new RawMessageReceivedEventArgs(data, client.ConnectionId, roomId));

        client.Disconnected += (_, reason) =>
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, client.ConnectionId, false, reason));

        clients[roomId] = client;
        var connectionId = await client.ConnectAsync(timeoutMs, cancellationToken);

        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, connectionId, true));
        return connectionId;
    }

    async Task ITransport.DisconnectAsync(Guid roomId, Guid connectionId)
    {
        if (clients.TryGetValue(roomId, out var client) && client.ConnectionId == connectionId)
        {
            await client.DisconnectAsync();
            clients.TryRemove(roomId, out _);
        }
        else
        {
            logger.Log($"Попытка остоединиться от соеднинения {connectionId} для комнаты {roomId}, которой не нашлось");
        }
    }

    async Task ITransport.SendAsync(byte[] data, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
    {
        if (servers.TryGetValue(roomId, out var server))
        {
            await server.SendToConnectionAsync(data, connectionId, cancellationToken);
            return;
        }

        if (clients.TryGetValue(roomId, out var client) && client.ConnectionId == connectionId)
        {
            await client.SendAsync(data, cancellationToken);
            return;
        }

        throw new InvalidOperationException($"Нет соединений для комнаты {roomId} с идентификатором {connectionId}");
    }

    async Task ITransport.BroadcastAsync(byte[] data, Guid roomId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken)
    {
        if (servers.TryGetValue(roomId, out var server))
        {
            await server.BroadcastAsync(data, excludeConnections, cancellationToken);
            return;
        }

        if (clients.TryGetValue(roomId, out var client))
        {
            await client.SendAsync(data, cancellationToken);
            return;
        }

        throw new InvalidOperationException($"Нет соединений для комнаты {roomId}");
    }
}
