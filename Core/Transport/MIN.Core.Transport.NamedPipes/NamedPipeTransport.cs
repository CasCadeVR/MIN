using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Client;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Core.Transport.NamedPipes.Server;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Transport.NamedPipes;

/// <summary>
/// Реализация транспорта на основе Named Pipes
/// </summary>
public sealed class NamedPipeTransport : ITransport
{
    private readonly IEndPointProvider endPointProvider;
    private readonly IConfiguration configuration;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, NamedPipeServer> servers = new();
    private readonly ConcurrentDictionary<Guid, NamedPipeClient> clients = new();

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeTransport"/>
    /// </summary>
    public NamedPipeTransport(IEndPointProvider endPointProvider,
        IConfiguration configuration,
        ILoggerProvider logger)
    {
        this.endPointProvider = endPointProvider;
        this.configuration = configuration;
        this.logger = logger;
    }

    /// <inheritdoc />
    public event EventHandler<RawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    async Task ITransport.StartHostingAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var endpoint = endPointProvider.CreateEndpointForRoom(roomId);

        if (endpoint is not NamedPipeEndpoint namedPipeEndpoint)
        {
            throw new ArgumentException("Endpoint must be NamedPipeEndpoint", nameof(endpoint));
        }

        if (servers.ContainsKey(roomId))
        {
            return;
        }

        var server = new NamedPipeServer(namedPipeEndpoint, configuration, logger);
        servers[roomId] = server;

        server.RawMessageReceived += (_, args) =>
            RawMessageReceived?.Invoke(this, new RawMessageReceivedEventArgs(args.Data, roomId, args.ConnectionId));

        server.ConnectionDisconnected += (_, args) =>
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, args.ConnectionId, false, args.Reason));

        await server.StartAsync(cancellationToken);
        logger.Log($"Сервер стартанул комнату с id {roomId} на pipe {namedPipeEndpoint.PipeName}");
    }

    IEndpoint ITransport.GetEndpoint(Guid roomId)
    {
        if (clients.TryGetValue(roomId, out var client))
        {
            return client.Endpoint;
        }
        else if (servers.TryGetValue(roomId, out var server))
        {
            return server.Endpoint;
        }
        else
        {
            throw new InvalidOperationException($"Нет соединений для комнаты {roomId}");
        }
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

        var client = new NamedPipeClient(namedPipeEndpoint, configuration, logger);
        var connectionId = await client.ConnectAsync(timeoutMs, cancellationToken);
        clients[roomId] = client;

        client.RawMessageReceived += (_, data) =>
            RawMessageReceived?.Invoke(this, new RawMessageReceivedEventArgs(data, roomId, connectionId));

        client.Disconnected += (_, reason) =>
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, connectionId, false, reason));

        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, connectionId, true));
        return connectionId;
    }

    async Task ITransport.DisconnectAsync(Guid roomId, Guid connectionId)
    {
        if (clients.TryGetValue(roomId, out var client) && client.ConnectionId == connectionId)
        {
            await client.DisconnectAsync();
            clients.TryRemove(roomId, out _);
            return;
        }

        logger.Log($"Попытка отсоединиться от соеднинения {connectionId} для комнаты {roomId}, которой не нашлось", LogLevel.Warning);
    }

    async Task ITransport.SendAsync(byte[] data, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
    {
        if (clients.TryGetValue(roomId, out var client) && client.ConnectionId == connectionId)
        {
            await client.SendAsync(data, cancellationToken);
        }
        else if (servers.TryGetValue(roomId, out var server))
        {
            await server.SendToConnectionAsync(data, connectionId, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"Нет соединений для комнаты {roomId} с идентификатором {connectionId}");
        }
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
