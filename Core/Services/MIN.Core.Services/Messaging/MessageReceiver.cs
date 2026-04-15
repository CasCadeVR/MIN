using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models.Constants;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Messaging;

/// <inheritdoc cref="IMessageReceiver"/>
public sealed class MessageReceiver : IMessageReceiver, IAsyncDisposable
{
    private readonly ITransport transport;
    private readonly IMessageSerializer serializer;
    private readonly IEventBus eventBus;
    private readonly IMessageDispatcher dispatcher;
    private readonly IMessageEncryptor encryptor;
    private readonly ILoggerProvider logger;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly IChunkBufferAssembler chunkBufferAssembler;
    private readonly IStreamManager streamManager;
    private CancellationTokenSource? cts;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="MessageReceiver"/>
    /// </summary>
    public MessageReceiver(ITransport transport,
        IMessageSerializer serializer,
        IEventBus eventBus,
        IMessageDispatcher dispatcher,
        IMessageEncryptor encryptor,
        ILoggerProvider logger,
        IParticipantConnectionRegistry participantConnectionRegistry,
        IChunkBufferAssembler chunkBufferAssembler,
        IStreamManager streamManager)
    {
        this.transport = transport;
        this.serializer = serializer;
        this.eventBus = eventBus;
        this.dispatcher = dispatcher;
        this.encryptor = encryptor;
        this.logger = logger;
        this.participantConnectionRegistry = participantConnectionRegistry;
        this.chunkBufferAssembler = chunkBufferAssembler;
        this.streamManager = streamManager;
    }

    async Task IMessageReceiver.StartListeningAsync(CancellationToken cancellationToken)
    {
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        transport.RawMessageReceived += OnRawMessageReceived;
        chunkBufferAssembler.MessageAssembled += OnMessageAssembled;
        chunkBufferAssembler.ChunkAckRequested += OnChunkAckRequested;
        eventBus.Subscribe<LocalMessageRecievedEvent>(OnLocalMessageRecieved);
        await Task.CompletedTask;
    }

    private async Task OnLocalMessageRecieved(LocalMessageRecievedEvent e, CancellationToken cancellationToken)
    {
        await dispatcher.DispatchAsync(e.Message,
            new MessageContext(e.SenderId,
            e.RoomId,
            CoreRegistryConstants.LocalConnectionId,
            cancellationToken));
    }

    private async void OnMessageAssembled(object? sender, MessageAssembledEventArgs e)
    {
        try
        {
            participantConnectionRegistry.TryGetParticipantFromConnectionId(e.ConnectionId, out var participantInfo);
            var message = serializer.Deserialize(e.Data);

            await dispatcher.DispatchAsync(message, new MessageContext(
                participantInfo?.Id ?? Guid.Empty,
                e.RoomId,
                e.ConnectionId,
                cts!.Token));
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при обработке собранного сообщения: {ex.Message}");
        }
    }

    private async void OnChunkAckRequested(object? sender, ChunkAckRequestedEventArgs e)
    {
        try
        {
            var ack = new byte[TransportConstants.ChunkAckSize];
            ack[0] = TransportConstants.ChunkAckMarker;
            e.StreamId.TryWriteBytes(new Span<byte>(ack, 1, 16));
            BitConverter.GetBytes(e.ChunkIndex).CopyTo(ack, 17);

            await transport.SendAsync(ack, e.RoomId, e.ConnectionId, cts!.Token);
            logger.Log($"Отправлен ACK для чанка {e.ChunkIndex} потока {e.StreamId}");
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при отправке ACK: {ex.Message}");
        }
    }

    private async void OnRawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
    {
        try
        {
            participantConnectionRegistry.TryGetParticipantFromConnectionId(e.ConnectionId, out var participantInfo);

            byte[] plainData;
            var body = encryptor.RemoveEncryptionHeader(e.Data);

            if (encryptor.IsEncrypted(e.Data) && e.ConnectionId != CoreRegistryConstants.LocalConnectionId)
            {
                if (participantInfo == null)
                {
                    logger.Log($"Получили зашифрованное сообщение от неизвестного соединения с id {e.ConnectionId}, игнорю");
                    return;
                }
                plainData = encryptor.DecryptMessage(body, participantInfo.Id);
            }
            else
            {
                plainData = body;
            }

            if (streamManager.IsAck(plainData))
            {
                streamManager.ProcessIncomingData(plainData);
                return;
            }

            if (IsStreamChunk(plainData))
            {
                ProcessStreamChunk(plainData, e.ConnectionId, e.RoomId, participantInfo);
                return;
            }

            var message = serializer.Deserialize(plainData);

            try
            {
                if (message is HandshakeAckMessage ackMessage)
                {
                    participantInfo = ackMessage.Participant;
                }

                await dispatcher.DispatchAsync(message, new MessageContext(participantInfo?.Id ?? Guid.Empty, e.RoomId, e.ConnectionId, cts!.Token));
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            logger.Log($"Произошла ошибка во время обработки raw message: {ex.Message}");
        }
    }

    private static bool IsStreamChunk(byte[] data)
    {
        if (data.Length < TransportConstants.StreamHeaderSize)
        {
            return false;
        }

        var flags = (StreamChunkFlags)data[0];
        return flags.HasFlag(StreamChunkFlags.Start) || flags.HasFlag(StreamChunkFlags.End) || flags.HasFlag(StreamChunkFlags.Mid);
    }

    private void ProcessStreamChunk(byte[] data, Guid connectionId, Guid roomId, ParticipantInfo? participantInfo)
    {
        if (participantInfo == null)
        {
            logger.Log($"Получен потоковый пакет от неизвестного соединения {connectionId}");
            return;
        }

        var chunk = new StreamChunk
        {
            StreamId = new Guid(data.AsSpan(1, 16)),
            Flags = (StreamChunkFlags)data[0],
            Index = BitConverter.ToInt32(data, 17),
            Total = BitConverter.ToInt32(data, 21),
            Data = new ReadOnlyMemory<byte>(data, TransportConstants.StreamHeaderSize, data.Length - TransportConstants.StreamHeaderSize)
        };

        chunkBufferAssembler.AddChunk(chunk, connectionId, roomId, requiresAcks: true);
    }

    /// <inheritdoc />
    public async Task StopListeningAsync()
    {
        transport.RawMessageReceived -= OnRawMessageReceived;
        chunkBufferAssembler.MessageAssembled -= OnMessageAssembled;
        chunkBufferAssembler.ChunkAckRequested -= OnChunkAckRequested;
        cts?.Cancel();
        cts?.Dispose();
        await Task.CompletedTask;
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await StopListeningAsync();
}
