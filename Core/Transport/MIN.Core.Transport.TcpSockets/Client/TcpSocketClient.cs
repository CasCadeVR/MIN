using System.Net.Sockets;
using MIN.Core.Transport.TcpSockets.Models;

namespace MIN.Core.Transport.TcpSockets.Client;

/// <summary>
/// Клиент Tcp для подключения к удалённой комнате
/// </summary>
internal sealed class TcpSocketClient : IAsyncDisposable
{
    private TcpClient? client;
    private TcpSocketConnection? connection;
    private bool disposed;

    /// <summary>
    /// Событие получения сообщения
    /// </summary>
    public event Action<byte[]>? OnMessageReceived;

    /// <summary>
    /// Событие отключения
    /// </summary>
    public event Action<string?>? OnDisconnected;

    /// <summary>
    /// Идентификатор соеднинения
    /// </summary>
    public Guid ConnectionId => connection?.Id ?? Guid.Empty;

    /// <summary>
    /// Флаг подключения
    /// </summary>
    public bool IsConnected => connection?.IsConnected == true;

    /// <summary>
    /// Подключиться к серверу
    /// </summary>
    public async Task<Guid> ConnectAsync(string ipAddress, int port, int timeoutMs)
    {
        client = new TcpClient();
        using var cts = new CancellationTokenSource(timeoutMs);
        await client.ConnectAsync(ipAddress, port, cts.Token);

        connection = new TcpSocketConnection(client);
        connection.RawMessageReceived += (_, msg) => OnMessageReceived?.Invoke(msg);
        connection.Disconnected += (_, ex) => OnDisconnected?.Invoke(ex);
        connection.StartReading();

        return connection.Id;
    }

    public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (connection == null)
        {
            throw new InvalidOperationException("Not connected");
        }
        await connection.SendAsync(data, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        if (connection != null)
        {
            await connection.DisposeAsync();
        }

        client?.Dispose();
    }
}
