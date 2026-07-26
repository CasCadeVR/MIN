using System.Collections.Concurrent;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Helpers;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models;
using MIN.Core.Transport.TcpSockets.Client;
using MIN.Core.Transport.TcpSockets.Models;
using MIN.Core.Transport.TcpSockets.Server;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Transport.TcpSockets;

/// <summary>
/// Реализация передачи данных на основе Tcp Socket
/// </summary>
public class TcpTransport : ITransport
{
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, TcpSocketServer> servers = new();
    private readonly ConcurrentDictionary<Guid, TcpSocketClient> clients = new();

    private IEnumerable<MachineKnownIp>? machineKnownIpsCache = [];

    /// <inheritdoc />
    public event EventHandler<RawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TcpTransport"/>
    /// </summary>
    public TcpTransport(ILoggerProvider logger)
    {
        this.logger = logger;
    }

    async Task<Guid> ITransport.StartHostingAsync(NetworkOptions networkOptions, CancellationToken cancellationToken)
    {
        var connectionId = Guid.NewGuid();
        var port = PortProvider.AllocatePort();
        var server = new TcpSocketServer(logger, port);

        server.OnMessageReceived += (TcpSocketServer server, (TcpSocketConnection conn, byte[] msg) eventArgs) =>
        {
            var args = new RawMessageReceivedEventArgs(eventArgs.msg, eventArgs.conn.Id, connectionId);
            RawMessageReceived?.Invoke(this, args);
        };

        server.OnConnectionEstablished += (s, conn) =>
        {
            var args = new ConnectionStateChangedEventArgs(conn.Id, true, serverConnectionId: connectionId)
            {
                RemoteEndPoint = conn.RemoteEndPoint,
            };
            ConnectionStateChanged?.Invoke(this, args);
        };

        server.ConnectionDisconnected += (TcpSocketServer server, (TcpSocketConnection conn, string? reason) eventArgs) =>
        {
            var args = new ConnectionStateChangedEventArgs(eventArgs.conn.Id, false, eventArgs.reason, connectionId);
            ConnectionStateChanged?.Invoke(this, args);
        };

        await server.StartAsync(networkOptions, cancellationToken);
        servers.TryAdd(connectionId, server);

        return connectionId;
    }

    async Task ITransport.StopHostingAsync(Guid connectionId)
    {
        if (servers.TryRemove(connectionId, out var server))
        {
            await server.DisposeAsync();
            PortProvider.ReleasePort(server.Port);
        }
    }

    async Task<Guid> ITransport.ConnectAsync(IEndpoint endpoint, CancellationToken cancellationToken)
    {
        if (endpoint is not TcpEndpoint tcpEp)
        {
            throw new ArgumentException("Endpoint must be TcpEndpoint");
        }

        var client = new TcpSocketClient();
        client.OnMessageReceived += msg =>
        {
            var args = new RawMessageReceivedEventArgs(msg, client.ConnectionId);
            RawMessageReceived?.Invoke(this, args);
        };
        client.OnDisconnected += reason =>
        {
            var args = new ConnectionStateChangedEventArgs(client.ConnectionId, false, reason);
            ConnectionStateChanged?.Invoke(this, args);
        };

        var connectionId = await client.ConnectAsync(tcpEp.IPAddress, tcpEp.Port, cancellationToken);
        clients.TryAdd(connectionId, client);

        var connectedArgs = new ConnectionStateChangedEventArgs(connectionId, true);
        ConnectionStateChanged?.Invoke(this, connectedArgs);

        return connectionId;
    }

    async Task ITransport.SendAsync(byte[] data, Guid receipientConnectionId, Guid? serverConnectionId, CancellationToken cancellationToken)
    {
        if (clients.TryGetValue(receipientConnectionId, out var client))
        {
            await client.SendAsync(data, cancellationToken);
            return;
        }

        if (servers.TryGetValue(serverConnectionId ?? Guid.Empty, out var server) &&
            server.Connections.TryGetValue(receipientConnectionId, out var conn))
        {
            await conn.SendAsync(data, cancellationToken);
            return;
        }

        logger.Log("Попытка отправить сообщение, не подключившись, игнорю", LogLevel.Warning);
    }

    async Task ITransport.BroadcastAsync(byte[] data, Guid connectionId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken)
    {
        var excludeSet = excludeConnections?.ToHashSet() ?? [];

        if (servers.TryGetValue(connectionId, out var server))
        {
            var tasks = server.Connections.Values
                .Where(conn => !excludeSet.Contains(conn.Id))
                .Select(conn => conn.SendAsync(data, cancellationToken));
            await Task.WhenAll(tasks);
        }
    }

    async Task<IEnumerable<IEndpoint>> ITransport.GetEndpoints(Guid connectionId)
    {
        if (!servers.TryGetValue(connectionId, out var server))
        {
            throw new InvalidOperationException($"Connection {connectionId} is not hosted locally");
        }

        var result = new List<TcpEndpoint>();

        machineKnownIpsCache ??= await NetworkHelper.GetAllKnownIpsAsync();

        foreach (var ip in machineKnownIpsCache)
        {
            result.Add(new TcpEndpoint
            {
                IpOrigin = ip.Origin,
                IPAddress = ip.ToString(),
                NetworkName = ip.NetworkName,
                Port = server.Port
            });
        }

        return result;
    }

    /// <inheritdoc />
    public async Task DisconnectClientAsync(Guid clientConnectionId, Guid? serverConnectionId)
    {
        logger.Log($"Отключаю соединения с id {clientConnectionId}");
        if (servers.TryGetValue(serverConnectionId ?? Guid.Empty, out var server) &&
            server.Connections.TryGetValue(clientConnectionId, out var conn))
        {
            await conn.DisposeAsync();
        }
        else if (clients.TryGetValue(clientConnectionId, out var client))
        {
            await client.DisposeAsync();
            clients.TryRemove(clientConnectionId, out _);
        }
    }

    async Task ITransport.DisconnectAsync(Guid connectionId)
    {
        await DisconnectClientAsync(connectionId, null);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public async ValueTask DisposeAsync()
    {
        foreach (var server in servers.Values)
        {
            await server.DisposeAsync();
        }
        foreach (var client in clients.Values)
        {
            await client.DisposeAsync();
        }
    }
}
