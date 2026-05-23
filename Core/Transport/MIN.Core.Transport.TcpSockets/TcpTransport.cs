using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.TcpSockets.Client;
using MIN.Core.Transport.TcpSockets.Models;
using MIN.Core.Transport.TcpSockets.Server;
using MIN.Core.Transport.TcpSockets.Services;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Transport.TcpSockets;

/// <summary>
/// Реализация передачи данных на основе Tcp Socket
/// </summary>
public class TcpTransport : ITransport
{
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, TcpSocketServer> servers = new();
    private readonly ConcurrentDictionary<Guid, TcpSocketClient> clients = new();
    private readonly RoomPortManager portManager = new();

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

    async Task ITransport.StartHostingAsync(Guid roomId, CancellationToken cancellationToken)
    {
        if (servers.ContainsKey(roomId))
        {
            throw new InvalidOperationException($"Room {roomId} is already hosted");
        }

        var port = portManager.AllocatePort();
        var server = new TcpSocketServer(logger, port);

        server.OnMessageReceived += (TcpSocketServer server, (TcpSocketConnection conn, byte[] msg) eventArgs) =>
        {
            var args = new RawMessageReceivedEventArgs(eventArgs.msg, roomId, eventArgs.conn.Id);
            RawMessageReceived?.Invoke(this, args);
        };

        server.OnConnectionEstablished += (s, conn) =>
        {
            var args = new ConnectionStateChangedEventArgs(roomId, conn.Id, true);
            ConnectionStateChanged?.Invoke(this, args);
        };

        server.ConnectionDisconnected += (TcpSocketServer server, (TcpSocketConnection conn, string? reason) eventArgs) =>
        {
            var args = new ConnectionStateChangedEventArgs(roomId, eventArgs.conn.Id, false, eventArgs.reason);
            ConnectionStateChanged?.Invoke(this, args);
        };

        await server.StartAsync();
        servers.TryAdd(roomId, server);
    }

    async Task ITransport.StopHostingAsync(Guid roomId)
    {
        if (servers.TryRemove(roomId, out var server))
        {
            await server.DisposeAsync();
            portManager.ReleasePort(server.Port);
        }
    }

    async Task<Guid> ITransport.ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken)
    {
        if (endpoint is not TcpEndpoint tcpEp)
        {
            throw new ArgumentException("Endpoint must be TcpEndpoint");
        }

        var client = new TcpSocketClient();
        client.OnMessageReceived += msg =>
        {
            var args = new RawMessageReceivedEventArgs(msg, roomId, client.ConnectionId);
            RawMessageReceived?.Invoke(this, args);
        };
        client.OnDisconnected += reason =>
        {
            var args = new ConnectionStateChangedEventArgs(roomId, client.ConnectionId, false, reason);
            ConnectionStateChanged?.Invoke(this, args);
        };

        var connectionId = await client.ConnectAsync(tcpEp.IPAddress, tcpEp.Port, timeoutMs);
        clients.TryAdd(connectionId, client);

        var connectedArgs = new ConnectionStateChangedEventArgs(roomId, connectionId, true);
        ConnectionStateChanged?.Invoke(this, connectedArgs);

        return connectionId;
    }

    async Task ITransport.SendAsync(byte[] data, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
    {
        if (clients.TryGetValue(connectionId, out var client))
        {
            await client.SendAsync(data, cancellationToken);
            return;
        }

        if (servers.TryGetValue(roomId, out var server) &&
            server.Connections.TryGetValue(connectionId, out var conn))
        {
            await conn.SendAsync(data, cancellationToken);
            return;
        }

        throw new KeyNotFoundException($"Connection {connectionId} not found in room {roomId}");
    }

    async Task ITransport.BroadcastAsync(byte[] data, Guid roomId, IEnumerable<Guid>? excludeConnections, CancellationToken cancellationToken)
    {
        var excludeSet = excludeConnections?.ToHashSet() ?? [];

        if (servers.TryGetValue(roomId, out var server))
        {
            var tasks = server.Connections.Values
                .Where(conn => !excludeSet.Contains(conn.Id))
                .Select(conn => conn.SendAsync(data, cancellationToken));
            await Task.WhenAll(tasks);
        }
    }

    IEndpoint ITransport.GetEndpoint(Guid roomId)
    {
        if (!servers.TryGetValue(roomId, out var server))
        {
            throw new InvalidOperationException($"Room {roomId} is not hosted locally");
        }

        var localIp = GetLocalIpAddress();
        return new TcpEndpoint { IPAddress = localIp, Port = server.Port };
    }

    /// <inheritdoc />
    public async Task DisconnectClientAsync(Guid roomId, Guid connectionId, string reason)
    {
        if (servers.TryGetValue(roomId, out var server) &&
            server.Connections.TryGetValue(connectionId, out var conn))
        {
            await conn.DisposeAsync();
        }
        else if (clients.TryGetValue(connectionId, out var client))
        {
            await client.DisposeAsync();
            clients.TryRemove(connectionId, out _);
        }

        var args = new ConnectionStateChangedEventArgs(roomId, connectionId, false, reason);
        ConnectionStateChanged?.Invoke(this, args);
    }

    async Task ITransport.DisconnectAsync(Guid roomId, Guid connectionId)
    {
        await DisconnectClientAsync(roomId, connectionId, "Disconnected by user");
    }

    private static string GetLocalIpAddress()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        return endPoint?.Address.ToString() ?? "127.0.0.1";
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

        portManager.Dispose();
    }
}
