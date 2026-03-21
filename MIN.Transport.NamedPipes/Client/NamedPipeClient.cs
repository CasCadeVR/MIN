using System.IO.Pipes;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Stateless;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Endpoints;
using MIN.Transport.NamedPipes.Models;

namespace MIN.Transport.NamedPipes.Client;

/// <summary>
/// Клиент Named Pipe для подключения к удалённой комнате
/// </summary>
internal sealed class NamedPipeClient : IAsyncDisposable
{
    private readonly NamedPipeEndpoint endpoint;
    private readonly ParticipantInfo localParticipant;
    private readonly IMessageSerializer serializer;
    private readonly ICryptoProvider cryptoProvider;
    private readonly ILoggerProvider logger;
    private NamedPipeClientStream? pipe;
    private NamedPipeConnection? connection;
    private CancellationTokenSource? cts;
    private bool isConnected;

    public NamedPipeClient(
        NamedPipeEndpoint endpoint,
        ParticipantInfo localParticipant,
        IMessageSerializer serializer,
        ICryptoProvider cryptoProvider,
        ILoggerProvider logger)
    {
        this.endpoint = endpoint;
        this.localParticipant = localParticipant;
        this.serializer = serializer;
        this.cryptoProvider = cryptoProvider;
        this.logger = logger;
    }

    /// <summary>
    /// Событие получения сообщения
    /// </summary>
    public event EventHandler<byte[]>? MessageReceived;

    /// <summary>
    /// Событие отключения
    /// </summary>
    public event EventHandler<string?>? Disconnected;

    /// <summary>
    /// Информация об участнике-сервере (устанавливается после handshake)
    /// </summary>
    public ParticipantInfo? ServerParticipant { get; private set; }

    /// <summary>
    /// Флаг подключения
    /// </summary>
    public bool IsConnected => isConnected && pipe?.IsConnected == true;

    /// <summary>
    /// Подключается к серверу и выполняет handshake
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (isConnected)
        {
            return;
        }

        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        pipe = new NamedPipeClientStream(
            endpoint.MachineName,
            endpoint.PipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous | PipeOptions.WriteThrough);

        try
        {
            await pipe.ConnectAsync(5000, cancellationToken); // таймаут
            connection = new NamedPipeConnection(pipe, endpoint);
            connection.MessageReceived += OnMessageReceived;
            connection.Disconnected += OnDisconnected;

            await connection.StartReadingAsync(cts.Token);
            await PerformHandshakeAsync(cancellationToken);

            isConnected = true;
        }
        catch (Exception ex)
        {
            logger.Log($"Failed to connect: {ex.Message}");
            await DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Отправляет сообщение
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
    /// Отключается
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
    }

    private async Task PerformHandshakeAsync(CancellationToken cancellationToken)
    {
        // Отправляем HandshakeMessage
        var handshake = await cryptoProvider.CreateHandshakeAsync(localParticipant);
        var handshakeData = serializer.Serialize(handshake);
        await connection!.SendAsync(handshakeData, cancellationToken);

        // Ждём HandshakeAckMessage
        var data = await ReadMessageAsync(connection, cancellationToken);
        var ackMessage = serializer.Deserialize(data);

        if (ackMessage is not HandshakeAckMessage ack)
        {
            throw new InvalidOperationException("Expected HandshakeAckMessage");
        }

        // Устанавливаем общий секрет
        await cryptoProvider.InitializeSessionAsync(localParticipant.Id, ack.PublicKey);

        // Сохраняем информацию о сервере
        ServerParticipant = new ParticipantInfo(localParticipant.Id, "Server");
    }

    private void OnMessageReceived(object? sender, byte[] data)
    {
        // Расшифровываем при необходимости
        byte[] plainData;
        if (IsEncrypted(data))
        {
            var encrypted = RemoveHeader(data);
            plainData = cryptoProvider.DecryptMessage(encrypted, ServerParticipant!.Id);
        }
        else
        {
            plainData = RemoveHeader(data);
        }

        MessageReceived?.Invoke(this, plainData);
    }

    private void OnDisconnected(object? sender, string? reason)
    {
        isConnected = false;
        Disconnected?.Invoke(this, reason);
    }

    private async Task<byte[]> ReadMessageAsync(NamedPipeConnection connection, CancellationToken cancellationToken)
    {
        // Аналогично серверу – упрощённо
        var buffer = new byte[4096];
        var bytesRead = await connection.Pipe.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        var data = new byte[bytesRead];
        Array.Copy(buffer, data, bytesRead);
        return data;
    }

    private bool IsEncrypted(byte[] data) => data.Length > 0 && (data[0] & 0x01) != 0;
    private byte[] RemoveHeader(byte[] data) => data.Skip(1).ToArray();

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public async ValueTask DisposeAsync() => await DisconnectAsync();
}
