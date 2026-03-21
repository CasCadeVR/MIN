using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Entities.Contracts;
using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Stateless;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Endpoints;
using MIN.Transport.NamedPipes.Models;

namespace MIN.Transport.NamedPipes.Server;

/// <summary>
/// Сервер Named Pipe для комнаты
/// </summary>
internal sealed class NamedPipeServer : IAsyncDisposable
{
    private readonly Guid roomId;
    private readonly NamedPipeEndpoint endpoint;
    private readonly IMessageSerializer serializer;
    private readonly ICryptoProvider cryptoProvider;
    private readonly ILoggerProvider logger;
    private readonly SemaphoreSlim connectionSlots;
    private readonly List<NamedPipeConnection> connections = new();
    private CancellationTokenSource? cts;
    private bool isRunning;

    public NamedPipeServer(
        Guid roomId,
        NamedPipeEndpoint endpoint,
        int maxParticipants,
        IMessageSerializer serializer,
        ICryptoProvider cryptoProvider,
        ILoggerProvider logger)
    {
        this.roomId = roomId;
        this.endpoint = endpoint;
        this.serializer = serializer;
        this.cryptoProvider = cryptoProvider;
        this.logger = logger;
        connectionSlots = new SemaphoreSlim(maxParticipants, maxParticipants);
    }

    /// <summary>
    /// Событие, возникающее при получении сообщения от участника
    /// </summary>
    public event EventHandler<(byte[] Data, ParticipantInfo Sender)>? MessageReceived;

    /// <summary>
    /// Событие, возникающее при подключении нового участника (после handshake)
    /// </summary>
    public event EventHandler<ParticipantInfo>? ParticipantConnected;

    /// <summary>
    /// Событие, возникающее при отключении участника
    /// </summary>
    public event EventHandler<ParticipantInfo>? ParticipantDisconnected;

    /// <summary>
    /// Запускает сервер
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
    /// Останавливает сервер и закрывает все соединения
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
    /// Отправляет сообщение конкретному участнику
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
    /// Рассылает сообщение всем участникам (кроме исключённого)
    /// </summary>
    public async Task BroadcastAsync(byte[] data, ParticipantInfo? exclude = null, CancellationToken cancellationToken = default)
    {
        var tasks = connections
            .Where(c => exclude == null || c.Participant?.Id != exclude.Id)
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
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                    0, 0,
                    pipeSecurity);

                await pipe.WaitForConnectionAsync(cancellationToken);

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
                logger.Log($"Error accepting client: {ex.Message}");
                connectionSlots.Release();
            }
        }
    }

    private async Task HandleConnectionAsync(NamedPipeConnection connection, CancellationToken cancellationToken)
    {
        connection.MessageReceived += async (_, data) =>
        {
            await OnConnectionMessageReceivedAsync(connection, data, cancellationToken);
        };
        connection.Disconnected += (_, reason) =>
        {
            OnConnectionDisconnected(connection, reason);
        };

        await connection.StartReadingAsync(cancellationToken);

        // Выполняем handshake
        try
        {
            await PerformHandshakeAsync(connection, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Log($"Handshake failed: {ex.Message}");
            await connection.DisposeAsync();
        }
    }

    private async Task PerformHandshakeAsync(NamedPipeConnection connection, CancellationToken cancellationToken)
    {
        // Ждём HandshakeMessage
        var data = await ReadMessageAsync(connection, cancellationToken);
        var message = serializer.Deserialize(data);

        if (message is not HandshakeMessage handshake)
        {
            throw new InvalidOperationException("Expected HandshakeMessage");
        }

        // Устанавливаем общий секрет
        await cryptoProvider.InitializeSessionAsync(handshake.Participant.Id, handshake.PublicKey);

        var selfHandshake = await cryptoProvider.CreateHandshakeAsync(null!);

        // Отправляем HandshakeAckMessage
        var ack = new HandshakeAckMessage
        {
            PublicKey = selfHandshake.PublicKey
        };
        var ackData = serializer.Serialize(ack);
        await connection.SendAsync(ackData, cancellationToken);

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

        // Расшифровываем при необходимости (если заголовок указывает на шифрование)
        byte[] plainData;
        if (IsEncrypted(data))
        {
            var encrypted = RemoveHeader(data);
            plainData = cryptoProvider.DecryptMessage(encrypted, connection.Participant.Id);
        }
        else
        {
            plainData = RemoveHeader(data);
        }

        MessageReceived?.Invoke(this, (plainData, connection.Participant));
    }

    private void OnConnectionDisconnected(NamedPipeConnection connection, string? reason)
    {
        connections.Remove(connection);
        connectionSlots.Release();

        if (connection.Participant != null)
        {
            ParticipantDisconnected?.Invoke(this, connection.Participant);
        }
    }

    private async Task<byte[]> ReadMessageAsync(NamedPipeConnection connection, CancellationToken cancellationToken)
    {
        // Здесь нужно реализовать чтение с заголовком длины или фиксированным буфером.
        // Для простоты пока предполагаем, что сообщение целиком помещается в буфер.
        // В реальности нужно читать заголовок длины.
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
    public async ValueTask DisposeAsync() => await StopAsync();
}
