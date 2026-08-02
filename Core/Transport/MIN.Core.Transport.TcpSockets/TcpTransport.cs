using System.Collections.Concurrent;
using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Helpers;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models;
using MIN.Core.Transport.TcpSockets.Client;
using MIN.Core.Transport.TcpSockets.Models;
using MIN.Core.Transport.TcpSockets.Server;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;
using Open.Nat;

namespace MIN.Core.Transport.TcpSockets;

/// <summary>
/// Реализация передачи данных на основе Tcp Socket
/// </summary>
public class TcpTransport : ITransport
{
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, TcpSocketServer> servers = new();
    private readonly ConcurrentDictionary<Guid, TcpSocketClient> clients = new();

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

    async Task<Guid> ITransport.StartHostingAsync(CancellationToken cancellationToken)
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

        server.ConnectionDisconnected += (TcpSocketServer server, (TcpSocketConnection conn, DisconnectReason reason) eventArgs) =>
        {
            var args = new ConnectionStateChangedEventArgs(eventArgs.conn.Id, false, eventArgs.reason, connectionId);
            ConnectionStateChanged?.Invoke(this, args);
        };

        await server.StartAsync(cancellationToken);
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

        var client = new TcpSocketClient(logger);
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

    async Task<IEnumerable<IEndpoint>> ITransport.SetUpAndGetEndpoints(Guid connectionId, NetworkOptions networkOptions, NetworkOptions? oldNetworkOptions, CancellationToken cancellationToken)
    {
        if (!servers.TryGetValue(connectionId, out var server))
        {
            throw new InvalidOperationException($"Connection {connectionId} is not hosted locally");
        }

        var includeWan = false;

        if ((oldNetworkOptions == null && networkOptions.EnablePortForwarding)
            || (oldNetworkOptions.HasValue && networkOptions.EnablePortForwarding && !oldNetworkOptions.Value.EnablePortForwarding))
        {
            includeWan = true;

            ResultCodes? result = ResultCodes.UNKNOWN_ERROR;
            try
            {
                result = await PortForwardingHelper.MapPortAsync(server.Port, Protocol.Tcp, cancellationToken, $"Room {server.Port}");
            }
            catch
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            switch (result)
            {
                case ResultCodes.SUCCESS:
                    logger.Log($"Порт проброшен. Публичный порт: {server.Port}");
                    break;

                case ResultCodes.CONFLICT_IN_MAPPING_ENTRY:
                    var conflictMessage = "UPnP конфликтует с текущим портом. Клиенты из Интернета не могут подключиться, если порт не проброшен вручную. Попробуйте повторить попытку";
                    logger.Log(conflictMessage, LogLevel.Error);
                    throw new InvalidOperationException(conflictMessage);

                case ResultCodes.UNKNOWN_ERROR:
                    var message = "UPnP не доступен. Клиенты из Интернета не могут подключиться, если порт не проброшен вручную. Либо ну удалось получить публичный адрес.";
                    logger.Log(message, LogLevel.Error);
                    throw new InvalidOperationException(message);
            }
        }
        else if (oldNetworkOptions.HasValue && !networkOptions.EnablePortForwarding && oldNetworkOptions.Value.EnablePortForwarding)
        {
            await PortForwardingHelper.UnmapPortAsync(server.Port, Protocol.Tcp, cancellationToken);
        }

        var includeVpns = oldNetworkOptions == null && networkOptions.EnableRadmin
            || oldNetworkOptions.HasValue && networkOptions.EnableRadmin && !oldNetworkOptions.Value.EnableRadmin;

        IEnumerable<MachineKnownIp> knownIps = [];

        try
        {
            knownIps = await NetworkHelper.GetAllKnownIpsAsync(includeWan, includeVpns, cancellationToken);
        }
        catch
        {
            if ((oldNetworkOptions == null && networkOptions.EnablePortForwarding)
                || (oldNetworkOptions.HasValue && networkOptions.EnablePortForwarding && !oldNetworkOptions.Value.EnablePortForwarding))
            {
                await PortForwardingHelper.UnmapPortAsync(server.Port, Protocol.Tcp, cancellationToken);
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        var endpoints = new List<TcpEndpoint>();

        foreach (var ip in knownIps)
        {
            endpoints.Add(new TcpEndpoint
            {
                Origin = ip.Origin,
                IPAddress = ip.Address.ToString(),
                NetworkName = ip.NetworkName,
                Port = server.Port
            });
        }

        return endpoints;
    }

    /// <inheritdoc />
    public async Task DisconnectClientAsync(Guid clientConnectionId, Guid? serverConnectionId, DisconnectReason reason = DisconnectReason.None)
    {
        logger.Log($"Отключаю соединения с id {clientConnectionId}");
        if (servers.TryGetValue(serverConnectionId ?? Guid.Empty, out var server) &&
            server.Connections.TryGetValue(clientConnectionId, out var conn))
        {
            await conn.StopAsync(reason);
        }
        else if (clients.TryGetValue(clientConnectionId, out var client))
        {
            await client.StopAsync(reason);
            clients.TryRemove(clientConnectionId, out _);
        }
    }

    async Task ITransport.DisconnectAsync(Guid connectionId, DisconnectReason reason)
    {
        await DisconnectClientAsync(connectionId, null, reason);
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
