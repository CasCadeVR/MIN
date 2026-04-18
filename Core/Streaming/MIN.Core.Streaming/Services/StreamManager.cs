using System.Collections.Concurrent;
using MIN.Core.Cryptography.Contracts.Interfaces;
using MIN.Core.Headers.Contracts.Enums;
using MIN.Core.Headers.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Streaming.Contracts.Constants;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Streaming.Services;

/// <inheritdoc cref="IStreamManager"/>
public sealed class StreamManager : IStreamManager, IDisposable
{
    private readonly ITransport transport;
    private readonly IMessageEncryptor encryptor;
    private readonly IHeaderManager headerManager;
    private readonly IParticipantConnectionRegistry participantConnectionRegistry;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<ChunkAckKey, PendingChunk> pendingChunks = new();
    private readonly ConcurrentDictionary<ChunkAckKey, Timer> ackTimers = new();
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="StreamManager"/>
    /// </summary>
    public StreamManager(ITransport transport,
        IMessageEncryptor encryptor,
        IHeaderManager headerManager,
        IParticipantConnectionRegistry participantConnectionRegistry,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.encryptor = encryptor;
        this.headerManager = headerManager;
        this.participantConnectionRegistry = participantConnectionRegistry;
        this.logger = logger;
    }

    async Task IStreamManager.SendAsync(ReadOnlyMemory<byte> data,
        StreamOptions options,
        Guid roomId,
        Guid recipientConnectionId,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(disposed, nameof(StreamManager));

        var streamId = Guid.NewGuid();
        var totalChunks = (int)Math.Ceiling((double)data.Length / StreamingConstants.ChunkDataSize);

        logger.Log($"Начало отправки потока {streamId}: {data.Length} байт, {totalChunks} пакетов");

        for (var i = 0; i < totalChunks; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var chunkStart = i * StreamingConstants.ChunkDataSize;
            var chunkLength = Math.Min(StreamingConstants.ChunkDataSize, data.Length - chunkStart);
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

            if (options.RequiresAcks)
            {
                var ackKey = new ChunkAckKey { StreamId = streamId, ChunkIndex = i };
                pendingChunks.TryAdd(ackKey, new PendingChunk(i, totalChunks));
                StartAckTimer(ackKey);
            }

            logger.Log($"Отправлен пакет {i + 1}/{totalChunks} для потока {streamId}");

            await transport.SendAsync(encrypted, roomId, recipientConnectionId, cancellationToken);
        }
    }

    void IStreamManager.ProcessAck(byte[] data)
    {
        if (!headerManager.IsAck(data))
        {
            return;
        }

        var streamId = new Guid(data.AsSpan(1, 16));
        var chunkIndex = BitConverter.ToInt32(data, 17);
        var ackKey = new ChunkAckKey { StreamId = streamId, ChunkIndex = chunkIndex };

        OnChunkAcknowledged(ackKey);
        logger.Log($"Получен ACK для пакета {chunkIndex} потока {streamId}");
    }

    private void OnChunkAcknowledged(ChunkAckKey ackKey)
    {
        if (ackTimers.TryRemove(ackKey, out var timer))
        {
            timer.Dispose();
        }

        if (pendingChunks.TryGetValue(ackKey, out var pending))
        {
            pending.LastAcknowledgedIndex = ackKey.ChunkIndex;
        }
    }

    private void StartAckTimer(ChunkAckKey ackKey)
    {
        var timer = new Timer(
            OnAckTimeout,
            ackKey,
            StreamingConstants.DefaultChunkTimeoutMs,
            Timeout.InfiniteTimeSpan.Seconds);

        ackTimers.TryAdd(ackKey, timer);
    }

    private void OnAckTimeout(object? state)
    {
        if (state is ChunkAckKey ackKey)
        {
            logger.Log($"Таймаут ожидания ACK для пакета {ackKey.ChunkIndex}");
        }
    }

    private byte[] SerializeChunk(StreamChunk chunk)
    {
        var header = headerManager.BuildStreamChunkHeader(chunk.Flags, chunk.StreamId, chunk.Index, chunk.Total);
        var result = new byte[header.Length + chunk.Data.Length];
        header.CopyTo(result, 0);
        chunk.Data.CopyTo(result.AsMemory(header.Length));
        return result;
    }

    private byte[] EncryptChunkIfNeeded(byte[] plainData, Guid recipientConnectionId, StreamOptions options)
    {
        byte[] resultBytes;
        if (options.RequiresEncryption)
        {
            var recipientId = participantConnectionRegistry.GetParticipantIdFromConnectionId(recipientConnectionId);
            var encrypted = encryptor.EncryptMessage(plainData, recipientId);
            resultBytes = headerManager.AddHeader(encrypted, (byte)HeaderMessageType.Encrypted);
        }
        else
        {
            resultBytes = headerManager.AddHeader(plainData, (byte)HeaderMessageType.Plain);
        }
        return resultBytes;
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
