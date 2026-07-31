using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Headers.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Events;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Messaging;

/// <summary>
/// Сервис для обработки входящих по сети сообщений
/// </summary>
public sealed class MessageReceiver : IHostedService, IAsyncDisposable
{
    private readonly IRoomHoster roomHoster;
    private readonly IRoomConnector roomConnector;
    private readonly IMessageSerializer serializer;
    private readonly IEventBus eventBus;
    private readonly IMessageDispatcher dispatcher;
    private readonly IMessageEncryptor encryptor;
    private readonly IHeaderManager headerManager;
    private readonly IRoomFactory roomFactory;
    private readonly IChunkBufferAssembler chunkBufferAssembler;
    private readonly IStreamManager streamManager;
    private readonly ILoggerProvider logger;
    private CancellationTokenSource cts = null!;
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземлпяр <see cref="MessageReceiver"/>
    /// </summary>
    public MessageReceiver(IRoomHoster roomHoster,
        IRoomConnector roomConnector,
        IMessageSerializer serializer,
        IEventBus eventBus,
        IMessageDispatcher dispatcher,
        IMessageEncryptor encryptor,
        IHeaderManager headerManager,
        IRoomFactory roomFactory,
        IChunkBufferAssembler chunkBufferAssembler,
        IStreamManager streamManager,
        ILoggerProvider logger)
    {
        this.roomHoster = roomHoster;
        this.roomConnector = roomConnector;
        this.serializer = serializer;
        this.eventBus = eventBus;
        this.dispatcher = dispatcher;
        this.encryptor = encryptor;
        this.headerManager = headerManager;
        this.roomFactory = roomFactory;
        this.chunkBufferAssembler = chunkBufferAssembler;
        this.streamManager = streamManager;
        this.logger = logger;
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        roomHoster.RawMessageReceived += OnRawMessageReceived;
        roomConnector.RawMessageReceived += OnRawMessageReceived;
        chunkBufferAssembler.MessageAssembled += OnMessageAssembled;
        eventBus.Subscribe<LocalMessageRecievedEvent>(OnLocalMessageRecieved);
        await Task.CompletedTask;
    }

    private async Task OnLocalMessageRecieved(LocalMessageRecievedEvent e, CancellationToken cancellationToken)
    {
        var context = roomFactory.GetOrCreateContext(e.RoomId);
        await dispatcher.DispatchAsync(e.Message,
            new MessageContext(context,
            CoreRegistryConstants.LocalConnectionId,
            roomHoster.IsHosting(e.RoomId) ? Role.Host : Role.Client,
            cancellationToken), e.BroadcastExcludeIds);
    }

    private async void OnMessageAssembled(object? sender, MessageAssembledEventArgs e)
    {
        try
        {
            if (e.IsRawPayload)
            {
                return;
            }

            Guid? roomId = null;

            if (e.ServerConnectionId != null)
            {
                roomId = roomHoster.GetRoomIdByConnectionId(e.ServerConnectionId.Value);
            }
            else
            {
                roomId = roomConnector.GetRoomIdByConnectionId(e.ConnectionId);
            }

            var context = roomFactory.GetOrCreateContext(roomId.Value);
            var message = serializer.Deserialize(e.Data!); // Потому-что это не RawPayload
            await dispatcher.DispatchAsync(message, new MessageContext(context,
                e.ConnectionId,
                roomHoster.IsHosting(roomId.Value) ? Role.Host : Role.Client,
                cts.Token));
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при обработке собранного с потока сообщения: {ex.Message}");
        }
    }

    private async void OnRawMessageReceived(object? sender, RoomRawMessageReceivedEventArgs e)
    {
        try
        {
            if (headerManager.IsAck(e.Data))
            {
                streamManager.ProcessAck(e.Data);
                return;
            }

            var context = roomFactory.GetOrCreateContext(e.RoomId);

            context.Connections.TryGetParticipantFromConnectionId(e.ConnectionId, out var participantInfo);

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
                await chunkBufferAssembler.ProcessStreamChunk(plainData, e.ConnectionId, e.ServerConnectionId, cts.Token);
                return;
            }

            var actualData = plainData.AsSpan(1).ToArray();
            var message = serializer.Deserialize(actualData);

            try
            {
                await dispatcher.DispatchAsync(message, new MessageContext(context, e.ConnectionId, roomHoster.IsHosting(context.RoomId) ? Role.Host : Role.Client, cts.Token));
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
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        roomHoster.RawMessageReceived -= OnRawMessageReceived;
        roomConnector.RawMessageReceived -= OnRawMessageReceived;
        chunkBufferAssembler.MessageAssembled -= OnMessageAssembled;
        if (disposed)
        {
            return;
        }

        disposed = true;
        cts?.Cancel();
        cts?.Dispose();
        await Task.CompletedTask;
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync() => await StopAsync();
}
