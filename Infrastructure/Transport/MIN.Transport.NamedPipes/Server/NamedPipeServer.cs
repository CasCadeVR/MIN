using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Stateless;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Constants;
using MIN.Transport.NamedPipes.Models;

namespace MIN.Transport.NamedPipes.Server;

/// <summary>
/// Сервер Named Pipe для комнаты
/// </summary>
internal sealed class NamedPipeServer : IAsyncDisposable
{
    private readonly NamedPipeEndpoint endpoint;
    private readonly SemaphoreSlim connectionSlots;
    private readonly List<NamedPipeConnection> connections = new();

    private readonly IMessageSerializer serializer;
    private readonly IMessageEncryptor encryptor;
    private readonly ILoggerProvider logger;

    private CancellationTokenSource? cts;
    private bool isRunning;

    /// <summary>
    /// Инициализирует новый экзмепляр <see cref="NamedPipeServer"/>
    /// </summary>
    public NamedPipeServer(
        NamedPipeEndpoint endpoint,
        int maxParticipants,
        IMessageSerializer serializer,
        IMessageEncryptor encryptor,
        ILoggerProvider logger)
    {
        this.endpoint = endpoint;
        this.serializer = serializer;
        this.encryptor = encryptor;
        this.logger = logger;

        connectionSlots = new SemaphoreSlim(maxParticipants, maxParticipants);
    }

    /// <summary>
    /// Событие, возникающее при получении сообщения от участника
    /// </summary>
    public event EventHandler<(byte[] Data, ParticipantInfo Sender)>? RawMessageReceived;

    /// <summary>
    /// Событие, возникающее при подключении нового участника
    /// </summary>
    public event EventHandler<ParticipantInfo>? ParticipantConnected;

    /// <summary>
    /// Событие, возникающее при отключении участника
    /// </summary>
    public event EventHandler<(string reason, ParticipantInfo Sender)>? ParticipantDisconnected;

    /// <summary>
    /// Запустить сервер
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (isRunning)
        {
            return;
        }

        isRunning = true;
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await Task.Run(() => AcceptClientsAsync(cts.Token), cts.Token);
    }

    /// <summary>
    /// Останавить сервер и закрывает все соединения
    /// </summary>
    public async Task StopAsync()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        cts?.Cancel();

        var tasks = connections.Select(c => c.DisposeAsync().AsTask()).ToArray();
        await Task.WhenAll(tasks);
        connections.Clear();

        connectionSlots.Dispose();
    }

    /// <summary>
    /// Отправить сообщение конкретному участнику
    /// </summary>
    public async Task SendToParticipantAsync(byte[] data, ParticipantInfo target, CancellationToken cancellationToken = default)
    {
        var connection = connections.FirstOrDefault(c => c.Participant?.Id == target.Id);
        if (connection == null)
        {
            throw new InvalidOperationException($"Participant {target.Id} not connected");
        }

        await connection.SendAsync(data, cancellationToken);
    }

    /// <summary>
    /// Разослать сообщение всем участникам (кроме исключённого)
    /// </summary>
    public async Task BroadcastAsync(byte[] data, IEnumerable<ParticipantInfo>? toExclude = null, CancellationToken cancellationToken = default)
    {
        var excludeIds = toExclude?.Select(p => p.Id).ToHashSet() ?? [];

        var tasks = connections
            .Where(c => !excludeIds.Contains(c.Participant?.Id ?? Guid.Empty))
            .Select(c => c.SendAsync(data, cancellationToken))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("К сожалению, пока только на Windows");
        }

        var pipeSecurity = new PipeSecurity();
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
            AccessControlType.Allow));

        while (!cancellationToken.IsCancellationRequested && isRunning)
        {
            try
            {
                await connectionSlots.WaitAsync(cancellationToken);

                var pipe = NamedPipeServerStreamAcl.Create(
                    endpoint.PipeName,
                    PipeDirection.InOut,
                    TransportConstants.TheoraticallyPossibleMaximumRoomSize,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                    0, 0,
                    pipeSecurity);

                await pipe.WaitForConnectionAsync(cancellationToken);

                // Client connected here

                var connection = new NamedPipeConnection(pipe, endpoint);
                connections.Add(connection);

                _ = HandleConnectionAsync(connection, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время принятия клиента: {ex.Message}");
                connectionSlots.Release();
            }
        }
    }

    private async Task HandleConnectionAsync(NamedPipeConnection connection, CancellationToken cancellationToken)
    {
        connection.RawMessageReceived += async (_, data) =>
        {
            await OnConnectionMessageReceivedAsync(connection, data, cancellationToken);
        };
        connection.Disconnected += (_, reason) =>
        {
            OnConnectionDisconnected(connection, reason);
        };

        try
        {
            await PerformHandshakeAsync(connection, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Log($"Handshake не удался: {ex.Message}");
            await connection.DisposeAsync();
        }

        await connection.StartReadingAsync(cancellationToken);
    }

    private async Task PerformHandshakeAsync(NamedPipeConnection connection, CancellationToken cancellationToken)
    {
        var data = await ReadMessageAsync(connection, cancellationToken);
        var deserializedHandshake = serializer.Deserialize(data);

        if (deserializedHandshake is not HandshakeMessage handshake)
        {
            throw new InvalidOperationException($"Ожидали HandshakeMessage, получили {deserializedHandshake.GetType()}");
        }

        await encryptor.InitializeSessionWithPartnerAsync(handshake.Participant.Id, handshake.PublicKey);

        var selfHandshake = await encryptor.CreateSelfHandshakeMessageAsync(null!);

        // Отправляем HandshakeAckMessage
        var ack = new HandshakeAckMessage(selfHandshake);
        var serializedAck = serializer.Serialize(ack);
        await connection.SendAsync(serializedAck, cancellationToken);

        // Сохраняем участника
        var participant = new ParticipantInfo(handshake.Participant.Id, handshake.Participant.Name);
        connection.Participant = participant;

        ParticipantConnected?.Invoke(this, participant);
    }

    private async Task OnConnectionMessageReceivedAsync(NamedPipeConnection connection, byte[] data, CancellationToken cancellationToken)
    {
        if (connection.Participant == null)
        {
            // сообщение пришло до handshake – игнорируем (должно быть handshake)
            return;
        }

        byte[] plainData;

        if (encryptor.IsEncrypted(data))
        {
            var encrypted = encryptor.RemoveEncryptionHeader(data);
            plainData = encryptor.DecryptMessage(encrypted, connection.Participant.Id);
        }
        else
        {
            plainData = encryptor.RemoveEncryptionHeader(data);
        }

        RawMessageReceived?.Invoke(this, (plainData, connection.Participant));
    }

    private void OnConnectionDisconnected(NamedPipeConnection connection, string? reason)
    {
        connections.Remove(connection);
        connectionSlots.Release();

        if (connection.Participant != null)
        {
            ParticipantDisconnected?.Invoke(this, (reason ?? string.Empty, connection.Participant));
        }
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
    public async ValueTask DisposeAsync() => await StopAsync();
}
