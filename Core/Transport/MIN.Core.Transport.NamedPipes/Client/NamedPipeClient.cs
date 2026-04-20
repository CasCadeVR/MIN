using System.IO.Pipes;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Transport.NamedPipes.Client;

/// <summary>
/// Клиент Named Pipe для подключения к удалённой комнате
/// </summary>
internal sealed class NamedPipeClient : IAsyncDisposable
{
    private readonly NamedPipeEndpoint endpoint;
    private readonly ILoggerProvider logger;

    private NamedPipeConnection? connection;
    private CancellationTokenSource? cts;

    private bool isConnected;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeClient"/>
    /// </summary>
    public NamedPipeClient(
        NamedPipeEndpoint endpoint,
        ILoggerProvider logger)
    {
        this.endpoint = endpoint;
        this.logger = logger;
    }

    /// <summary>
    /// Событие получения сообщения
    /// </summary>
    public event EventHandler<byte[]>? RawMessageReceived;

    /// <summary>
    /// Событие отключения
    /// </summary>
    public event EventHandler<string?>? Disconnected;

    /// <summary>
    /// Точка подключения клиента
    /// </summary>
    public IEndpoint Endpoint => endpoint;

    /// <summary>
    /// Идентификатор соеднинения
    /// </summary>
    public Guid ConnectionId => connection?.Id ?? Guid.Empty;

    /// <summary>
    /// Флаг подключения
    /// </summary>
    public bool IsConnected => isConnected && connection?.Pipe.IsConnected == true;

    /// <summary>
    /// Подключиться к серверу и выполняет handshake
    /// </summary>
    public async Task<Guid> ConnectAsync(int timeoutMs = 1000, CancellationToken cancellationToken = default)
    {
        if (isConnected)
        {
            return ConnectionId;
        }

        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var pipe = new NamedPipeClientStream(
            endpoint.MachineName,
            endpoint.PipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous | PipeOptions.WriteThrough);

        try
        {
            await pipe.ConnectAsync(timeoutMs, cancellationToken);

            // Connected here

            connection = new NamedPipeConnection(pipe, endpoint);
            connection.RawMessageReceived += (_, data) => RawMessageReceived?.Invoke(this, data);
            connection.Disconnected += OnDisconnected;

            isConnected = true;
            _ = connection.StartReadingAsync(cts.Token);

            return connection.Id;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.Log($"Не получилось подключиться: {ex.Message}");
            await DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Отправить сообщение
    /// </summary>
    public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (!IsConnected || connection == null)
        {
            throw new InvalidOperationException("Not connected");
        }

        await connection.SendAsync(data, cancellationToken);
    }

    /// <summary>
    /// Отключиться
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (!isConnected)
        {
            return;
        }

        isConnected = false;
        cts?.Cancel();
        await (connection?.DisposeAsync() ?? ValueTask.CompletedTask);
        connection = null;
    }

    private void OnDisconnected(object? sender, string? reason)
    {
        isConnected = false;
        Disconnected?.Invoke(this, reason);
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await DisconnectAsync();
}
