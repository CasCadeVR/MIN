using System.IO.Pipes;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Stateless;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Constants;
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
    private readonly IMessageEncryptor encryptor;
    private readonly ILoggerProvider logger;

    private NamedPipeConnection? connection;
    private CancellationTokenSource? cts;

    private bool isConnected;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeClient"/>
    /// </summary>
    public NamedPipeClient(
        NamedPipeEndpoint endpoint,
        ParticipantInfo localParticipant,
        IMessageSerializer serializer,
        IMessageEncryptor encryptor,
        ILoggerProvider logger)
    {
        this.endpoint = endpoint;
        this.localParticipant = localParticipant;
        this.serializer = serializer;
        this.encryptor = encryptor;
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
    /// Информация об участнике-клиенте (устанавливается в конструкторе)
    /// </summary>
    public ParticipantInfo ClientParticipant => localParticipant;

    /// <summary>
    /// Информация об участнике-сервере (устанавливается после handshake)
    /// </summary>
    public ParticipantInfo? ServerParticipant { get; private set; }

    /// <summary>
    /// Флаг подключения
    /// </summary>
    public bool IsConnected => isConnected && connection?.Pipe.IsConnected == true;

    /// <summary>
    /// Подключиться к серверу и выполняет handshake
    /// </summary>
    public async Task ConnectAsync(int timeoutMs = 1000, CancellationToken cancellationToken = default)
    {
        if (isConnected)
        {
            return;
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
            connection.RawMessageReceived += OnMessageReceived;
            connection.Disconnected += OnDisconnected;

            await PerformHandshakeAsync(cancellationToken);
            await connection.StartReadingAsync(cts.Token);

            isConnected = true;
        }
        catch (Exception ex)
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
    }

    private async Task PerformHandshakeAsync(CancellationToken cancellationToken)
    {
        var handshake = await encryptor.CreateSelfHandshakeMessageAsync(localParticipant);
        var deserializedHandshake = serializer.Serialize(handshake);
        await connection!.SendAsync(deserializedHandshake, cancellationToken);

        var responseData = await ReadMessageAsync(connection, cancellationToken);
        var ackMessage = serializer.Deserialize(responseData);

        if (ackMessage is not HandshakeAckMessage ack)
        {
            throw new InvalidOperationException($"Expected HandshakeAckMessage, got {ackMessage.GetType()}");
        }

        await encryptor.InitializeSessionWithPartnerAsync(ack.Participant.Id, ack.PublicKey);
        ServerParticipant = ack.Participant;
    }

    private void OnMessageReceived(object? sender, byte[] data)
    {
        byte[] plainData;

        if (encryptor.IsEncrypted(data))
        {
            var encrypted = encryptor.RemoveEncryptionHeader(data);
            plainData = encryptor.DecryptMessage(encrypted, ServerParticipant!.Id);
        }
        else
        {
            plainData = encryptor.RemoveEncryptionHeader(data);
        }

        RawMessageReceived?.Invoke(this, plainData);
    }

    private void OnDisconnected(object? sender, string? reason)
    {
        isConnected = false;
        Disconnected?.Invoke(this, reason);
    }

    private async Task<byte[]> ReadMessageAsync(NamedPipeConnection connection, CancellationToken cancellationToken)
    {
        var buffer = new byte[TransportConstants.MaximumBufferSize];
        var bytesRead = await connection.Pipe.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        var data = new byte[bytesRead];
        Array.Copy(buffer, data, bytesRead);

        return data;
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public async ValueTask DisposeAsync() => await DisconnectAsync();
}
