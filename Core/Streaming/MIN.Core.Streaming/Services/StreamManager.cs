using System.Collections.Concurrent;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Models.Constants;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Streaming.Services;

/// <inheritdoc cref="IStreamManager"/>
public sealed class StreamManager : IStreamManager, IDisposable
{
    private readonly ITransport transport;
    private readonly IMessageEncryptor encryptor;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, PendingChunk> pendingChunks = new();
    private readonly ConcurrentDictionary<Guid, Timer> ackTimers = new();
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="StreamManager"/>
    /// </summary>
    public StreamManager(ITransport transport,
        IMessageEncryptor encryptor,
        IParticipantConnectionRegistry participantConnectionRegistry,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.encryptor = encryptor;
        this.participantConnectionRegistry = participantConnectionRegistry;
        this.logger = logger;
    }

    async Task<Guid> IStreamManager.SendAsync(ReadOnlyMemory<byte> data,
        StreamOptions options,
        Guid roomId,
        Guid recipientConnectionId,
        CancellationToken cancellationToken)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(StreamManager));
        }

        var streamId = options.StreamId;
        var totalChunks = (int)Math.Ceiling((double)data.Length / TransportConstants.ChunkDataSize);

        logger.Log($"Начало отправки потока {streamId}: {data.Length} байт, {totalChunks} пакетов");

        for (var i = 0; i < totalChunks; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var chunkStart = i * TransportConstants.ChunkDataSize;
            var chunkLength = Math.Min(TransportConstants.ChunkDataSize, data.Length - chunkStart);
            var chunkData = data.Slice(chunkStart, chunkLength);

            var flags = StreamChunkFlags.Mid;
            if (i == 0)
            {
                flags |= StreamChunkFlags.Start;
                if (options.RequiresAcks)
                {
                    flags |= StreamChunkFlags.RequiresAcks;
                }
            }
            if (i == totalChunks - 1)
            {
                flags |= StreamChunkFlags.End;
            }

            var chunk = new StreamChunk
            {
                StreamId = streamId,
                Flags = flags,
                Index = i,
                Total = totalChunks,
                Data = chunkData
            };

            var package = SerializeChunk(chunk);
            var encrypted = EncryptChunkIfNeeded(package, recipientConnectionId, options);

            await transport.SendAsync(encrypted, roomId, recipientConnectionId, cancellationToken);

            if (options.RequiresAcks)
            {
                pendingChunks.TryAdd(streamId, new PendingChunk(i, totalChunks));
                StartAckTimer(streamId, i, recipientConnectionId, options.ChunkTimeoutMs);
            }

            logger.Log($"Отправлен пакет {i + 1}/{totalChunks} для потока {streamId}");
        }

        return streamId;
    }

    void IStreamManager.ProcessAck(byte[] data)
    {
        if (!IsAck(data))
        {
            return;
        }

        var streamId = new Guid(data.AsSpan(1, 16));
        var chunkIndex = BitConverter.ToInt32(data, 17);

        OnChunkAcknowledged(streamId, chunkIndex);
        logger.Log($"Получен ACK для пакета {chunkIndex} потока {streamId}");
    }

    /// <inheritdoc />
    public bool IsAck(byte[] data)
        => data.Length >= TransportConstants.ChunkAckSize && data[0] == (byte)StreamChunkFlags.Ack;

    private void OnChunkAcknowledged(Guid streamId, int chunkIndex)
    {
        if (ackTimers.TryRemove(streamId, out var timer))
        {
            timer.Dispose();
        }

        if (pendingChunks.TryGetValue(streamId, out var pending))
        {
            pending.LastAcknowledgedIndex = chunkIndex;
        }
    }

    private static byte[] SerializeChunk(StreamChunk chunk)
    {
        var headerSize = TransportConstants.StreamHeaderSize;
        var result = new byte[headerSize + chunk.Data.Length];

        result[0] = (byte)chunk.Flags;
        chunk.StreamId.TryWriteBytes(new Span<byte>(result, 1, 16));
        BitConverter.GetBytes(chunk.Index).CopyTo(result, 17);
        BitConverter.GetBytes(chunk.Total).CopyTo(result, 21);

        chunk.Data.CopyTo(result.AsMemory(headerSize));

        return result;
    }

    private byte[] EncryptChunkIfNeeded(byte[] plainData, Guid recipientConnectionId, StreamOptions options)
    {
        byte[] resultBytes;
        if (options.RequiresEncryption)
        {
            var recipientId = participantConnectionRegistry.GetParticipantIdFromConnectionId(recipientConnectionId);
            var encrypted = encryptor.EncryptMessage(plainData, recipientId);
            resultBytes = encryptor.AddEncryptionHeader(encrypted);
        }
        else
        {
            resultBytes = encryptor.AddPlainHeader(plainData);
        }
        return resultBytes;
    }

    private void StartAckTimer(Guid streamId, int chunkIndex, Guid recipientConnectionId, int timeoutMs)
    {
        var timer = new Timer(
            OnAckTimeout,
            (streamId, chunkIndex, recipientConnectionId),
            timeoutMs,
            Timeout.InfiniteTimeSpan.Seconds);

        ackTimers.TryAdd(streamId, timer);
    }

    private void OnAckTimeout(object? state)
    {
        if (state is ValueTuple<Guid, int, Guid> args)
        {
            logger.Log($"Таймаут ожидания ACK для пакета {args.Item2} потока {args.Item1}");
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        foreach (var timer in ackTimers.Values)
        {
            timer.Dispose();
        }
        ackTimers.Clear();

        pendingChunks.Clear();
    }
}
