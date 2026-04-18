using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Headers.Contracts.Interfaces;
using MIN.Core.Messaging.Stateless;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
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
    private readonly IHeaderManager headerManager;
    private readonly ILoggerProvider logger;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly IChunkBufferAssembler chunkBufferAssembler;
    private readonly IStreamManager streamManager;
    private CancellationTokenSource cts = null!;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="MessageReceiver"/>
    /// </summary>
    public MessageReceiver(ITransport transport,
        IMessageSerializer serializer,
        IEventBus eventBus,
        IMessageDispatcher dispatcher,
        IMessageEncryptor encryptor,
        IHeaderManager headerManager,
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
        this.headerManager = headerManager;
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
                cts.Token));
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при обработке собранного с потока сообщения: {ex.Message}");
        }
    }

    private async void OnRawMessageReceived(object? sender, RawMessageReceivedEventArgs e)
    {
        try
        {
            if (headerManager.IsAck(e.Data))
            {
                streamManager.ProcessAck(e.Data);
                return;
            }

            participantConnectionRegistry.TryGetParticipantFromConnectionId(e.ConnectionId, out var participantInfo);

            byte[] plainData;
            var body = headerManager.RemoveEncryptionHeader(e.Data);

            if (headerManager.IsEncrypted(e.Data) && e.ConnectionId != CoreRegistryConstants.LocalConnectionId)
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

            if (headerManager.IsStreamChunk(plainData))
            {
                await chunkBufferAssembler.ProcessStreamChunk(plainData, e.ConnectionId, e.RoomId, cts.Token);
                return;
            }

            var actualData = plainData.AsSpan(1).ToArray();
            var message = serializer.Deserialize(actualData);

            try
            {
                if (message is HandshakeAckMessage ackMessage)
                {
                    participantInfo = ackMessage.Participant;
                }

                await dispatcher.DispatchAsync(message, new MessageContext(participantInfo?.Id ?? Guid.Empty, e.RoomId, e.ConnectionId, cts.Token));
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

    /// <inheritdoc />
    public async Task StopListeningAsync()
    {
        transport.RawMessageReceived -= OnRawMessageReceived;
        chunkBufferAssembler.MessageAssembled -= OnMessageAssembled;
        cts.Cancel();
        cts.Dispose();
        await Task.CompletedTask;
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await StopListeningAsync();
}
