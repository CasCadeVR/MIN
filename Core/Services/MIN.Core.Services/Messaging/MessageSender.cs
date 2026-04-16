using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models.Constants;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Services.Messaging;

/// <inheritdoc cref="IMessageSender"/>
public sealed class MessageSender : IMessageSender, IAsyncDisposable
{
    private readonly ITransport transport;
    private readonly IMessageEncryptor encryptor;
    private readonly IMessageSerializer serializer;
    private readonly IParticipantStore participantStore;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly IStreamManager streamManager;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инциализирует новый экземпляр <see cref="MessageSender"/>
    /// </summary>
    public MessageSender(ITransport transport,
        IMessageEncryptor encryptor,
        IMessageSerializer serializer,
        IParticipantStore participantStore,
        IParticipantConnectionRegistry participantConnectionRegistry,
        IStreamManager streamManager,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.encryptor = encryptor;
        this.serializer = serializer;
        this.participantStore = participantStore;
        this.participantConnectionRegistry = participantConnectionRegistry;
        this.streamManager = streamManager;
        this.logger = logger;
    }

    async Task IMessageSender.SendAsync(IMessage message, Guid roomId, Guid senderId, Guid recipientConnectionId, CancellationToken cancellationToken)
    {
        var serialized = serializer.Serialize(message);

        if (serialized.Length > TransportConstants.MessageBufferSize - TransportConstants.StreamHeaderSize)
        {
            var options = new StreamOptions
            {
                RequiresAcks = false,
                RequiresEncryption = message.RequiresEncryption
            };
            await streamManager.SendAsync(serialized.AsMemory(), options, roomId, recipientConnectionId, cancellationToken);
            return;
        }

        var dataToSend = EncryptDataIfRequired(message, serialized, recipientConnectionId);
        await transport.SendAsync(dataToSend, roomId, recipientConnectionId, cancellationToken);
    }

    async Task IMessageSender.BroadcastAsync(IMessage message, Guid roomId, IEnumerable<Guid>? excludeConnectionIds, CancellationToken cancellationToken)
    {
        var serialized = serializer.Serialize(message);
        var participants = participantStore.GetParticipants(roomId);

        excludeConnectionIds = (excludeConnectionIds ?? [])
            .Append(CoreRegistryConstants.LocalConnectionId);

        var tasks = participants
            .Select(participant => participantConnectionRegistry.GetConnectionIdFromParticipantId(participant.Id))
            .Where(connectionId => !excludeConnectionIds.Contains(connectionId))
            .Select(connectionId => transport.SendAsync(EncryptDataIfRequired(message, serialized, connectionId),
                roomId, connectionId, cancellationToken));

        await Task.WhenAll(tasks);
    }

    async Task<Guid> IMessageSender.SendLargeDataAsync(ReadOnlyMemory<byte> data, StreamOptions options, Guid roomId, Guid recipientConnectionId, CancellationToken cancellationToken)
    {
        return await streamManager.SendAsync(data, options, roomId, recipientConnectionId, cancellationToken);
    }

    private byte[] EncryptDataIfRequired(IMessage message, byte[] plainData, Guid recipientConnectionId)
    {
        var dataWithMarker = new byte[1 + plainData.Length];
        dataWithMarker[0] = (byte)StreamChunkFlags.RegularMessage;
        plainData.CopyTo(dataWithMarker, 1);

        byte[] resultBytes;
        if (message.RequiresEncryption)
        {
            var recipientId = participantConnectionRegistry.GetParticipantIdFromConnectionId(recipientConnectionId);
            var encrypted = encryptor.EncryptMessage(dataWithMarker, recipientId);
            resultBytes = encryptor.AddEncryptionHeader(encrypted);
        }
        else
        {
            resultBytes = encryptor.AddPlainHeader(dataWithMarker);
        }
        return resultBytes;
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
