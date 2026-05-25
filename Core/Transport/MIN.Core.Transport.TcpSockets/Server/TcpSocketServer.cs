using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using MIN.Core.Transport.Contracts.Constants;
using MIN.Core.Transport.TcpSockets.Models;
using MIN.Core.Transport.TcpSockets.Services;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Transport.TcpSockets.Server;

/// <summary>
/// Сервер Tcp Socket для комнаты
/// </summary>
internal sealed class TcpSocketServer : IAsyncDisposable
{
    private readonly ILoggerProvider logger;
    private readonly TcpListener listener;
    private readonly SemaphoreSlim connectionSlots;
    private readonly ConcurrentDictionary<Guid, TcpSocketConnection> connections = new();
    private readonly int maxConnections = TransportConstants.RoomMaximumConnectionsAmount;

    private CancellationTokenSource? cts;
    private PortForwardingHelper? pfHelper;
    private Task? acceptLoop;

    /// <summary>
    /// Ip Адрес подключения
    /// </summary>
    public IPAddress IpAddress = GetLocalIpAddress();

    /// <summary>
    /// Порт подключения
    /// </summary>
    public int Port => ((IPEndPoint)listener.LocalEndpoint).Port;

    /// <summary>
    /// Текущие соединения
    /// </summary>
    public IReadOnlyDictionary<Guid, TcpSocketConnection> Connections => connections;

    /// <summary>
    /// Инициализирует новый экзмепляр <see cref="TcpSocketServer"/>
    /// </summary>
    public TcpSocketServer(ILoggerProvider logger, int port)
    {
        this.logger = logger;
        listener = new TcpListener(IPAddress.Any, port);
        connectionSlots = new SemaphoreSlim(maxConnections, maxConnections);
    }

    /// <summary>
    /// Событие, возникающее при получении сообщения от соединения
    /// </summary>
    public event Action<TcpSocketServer, (TcpSocketConnection Connection, byte[] Message)>? OnMessageReceived;

    /// <summary>
    /// Соеднинение сервера и клиента разорвалось
    /// </summary>
    public event Action<TcpSocketServer, (TcpSocketConnection Connection, string? Reason)>? ConnectionDisconnected;

    /// <summary>
    /// Соеднинение сервера и клиента установилось
    /// </summary>
    public event Action<TcpSocketServer, TcpSocketConnection>? OnConnectionEstablished;

    /// <summary>
    /// Запустить сервер
    /// </summary>
    public async Task StartAsync(bool withPortForwarding, CancellationToken cancellationToken)
    {
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        listener.Start();
        acceptLoop = Task.Run(AcceptLoopAsync);
        logger.Log($"Стартанул сервер на порту: {Port}");

        if (withPortForwarding)
        {
            pfHelper = new PortForwardingHelper();
            var publicIp = await pfHelper.CreatePortForwardingAsync(IpAddress, Port, null, $"Room {Port}");
            if (publicIp != null)
            {
                IpAddress = publicIp;
                logger.Log($"Порт проброшен. Публичный IP: {publicIp}, Публичный порт: {Port}");
            }
            else
            {
                var message = "UPnP не доступен. Клиенты из Интернета не могут подключиться, если порт не проброшен вручную.";
                logger.Log(message, LogLevel.Error);
                throw new InvalidOperationException(message);
            }
        }

        await Task.CompletedTask;
    }

    private async Task AcceptLoopAsync()
    {
        try
        {
            while (!cts!.Token.IsCancellationRequested)
            {
                await connectionSlots.WaitAsync(cts.Token);
                var tcpClient = await listener.AcceptTcpClientAsync(cts.Token);
                _ = HandleConnectionAsync(tcpClient);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            logger.Log($"Произошла ошибка во время принятия клиента: {ex.Message}");
        }
    }

    private async Task HandleConnectionAsync(TcpClient tcpClient)
    {
        var connection = new TcpSocketConnection(tcpClient);
        logger.Log($"Клиент подключился: {connection.RemoteEndPoint ?? "unknown"}");
        try
        {
            connection.RawMessageReceived += OnConnectionMessage;
            connection.Disconnected += OnConnectionDisconnected;
            connection.StartReading();

            connections.TryAdd(connection.Id, connection);
            OnConnectionEstablished?.Invoke(this, connection);

            var tcs = new TaskCompletionSource<bool>();
            connection.Disconnected += (_, _) => tcs.TrySetResult(true);
            await tcs.Task;
        }
        finally
        {
            connections.TryRemove(connection.Id, out _);
            await connection.DisposeAsync();
            connectionSlots.Release();
        }
    }

    private void OnConnectionMessage(TcpSocketConnection conn, byte[] msg)
        => OnMessageReceived?.Invoke(this, (conn, msg));

    private void OnConnectionDisconnected(TcpSocketConnection conn, string? reason)
        => ConnectionDisconnected?.Invoke(this, (conn, reason));

    private static IPAddress GetLocalIpAddress()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        var endPoint = socket.LocalEndPoint as IPEndPoint;
        return endPoint?.Address ?? IPAddress.Parse("127.0.0.1");
    }

    public async ValueTask DisposeAsync()
    {
        cts?.Cancel();

        if (pfHelper != null)
        {
            await pfHelper.RemovePortForwardingAsync();
        }

        if (acceptLoop != null)
        {
            await acceptLoop.WaitAsync(TimeSpan.FromSeconds(5));
        }

        listener.Stop();

        foreach (var connection in connections.Values)
        {
            await connection.DisposeAsync();
        }

        connectionSlots.Dispose();
        cts?.Dispose();
    }
}
