using System.Collections.Concurrent;
using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.Core.Transport.Contracts.Models.Constants;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Streaming.Services;

/// <inheritdoc cref="IChunkBufferAssembler"/>
public sealed class ChunkBufferAssembler : IChunkBufferAssembler, IDisposable
{
    private readonly ConcurrentDictionary<Guid, MessageStream> activeStreams = new();
    private readonly ConcurrentDictionary<Guid, Timer> streamTimers = new();
    private readonly ILoggerProvider? logger;
    private bool disposed;

    /// <inheritdoc />
    public event EventHandler<MessageAssembledEventArgs>? MessageAssembled;

    /// <inheritdoc />
    public event EventHandler<ChunkAckRequestedEventArgs>? ChunkAckRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChunkBufferAssembler"/>
    /// </summary>
    public ChunkBufferAssembler(ILoggerProvider? logger = null)
    {
        this.logger = logger;
    }

    /// <inheritdoc />
    public void AddChunk(StreamChunk chunk, Guid connectionId, Guid roomId)
    {
        if (disposed)
        {
            return;
        }

        try
        {
            var requiresAcks = chunk.Flags.HasFlag(StreamChunkFlags.RequiresAcks);

            if (chunk.IsSingle)
            {
                if (requiresAcks)
                {
                    OnChunkAckRequested(chunk.StreamId, chunk.Index, connectionId, roomId);
                }

                OnMessageAssembled(chunk.StreamId, connectionId, roomId, chunk.Data.ToArray());
                return;
            }

            var stream = activeStreams.GetOrAdd(chunk.StreamId, _ => CreateMessageStream(chunk, connectionId, roomId, requiresAcks));

            if (stream.ConnectionId != connectionId)
            {
                logger?.Log($"Получен пакет для потока {chunk.StreamId} от другого соединения");
                return;
            }

            ResetStreamTimer(stream);

            if (chunk.Flags.HasFlag(StreamChunkFlags.Start))
            {
                stream.LastChunkReceivedAt = DateTime.UtcNow;
            }

            if (requiresAcks)
            {
                OnChunkAckRequested(chunk.StreamId, chunk.Index, connectionId, roomId);
            }

            var completeData = stream.AddChunk(chunk);
            if (completeData != null)
            {
                RemoveStream(chunk.StreamId);
                OnMessageAssembled(chunk.StreamId, connectionId, roomId, completeData);
            }
        }
        catch (Exception ex)
        {
            logger?.Log($"Ошибка при добавлении пакета: {ex.Message}");
            RemoveStream(chunk.StreamId);
            throw;
        }
    }

    Task IChunkBufferAssembler.AddChunkAsync(StreamChunk chunk, Guid connectionId, Guid roomId, CancellationToken cancellationToken)
    {
        return Task.Run(() => AddChunk(chunk, connectionId, roomId), cancellationToken);
    }

    private MessageStream CreateMessageStream(StreamChunk startChunk, Guid connectionId, Guid roomId, bool requiresAcks)
    {
        var stream = new MessageStream(startChunk.StreamId, connectionId, roomId, startChunk.Total, requiresAcks);
        StartStreamTimer(stream);
        return stream;
    }

    private void StartStreamTimer(MessageStream stream)
    {
        var timer = new Timer(
            OnStreamTimeout,
            stream.Id,
            stream.CreatedAt.AddMilliseconds(TransportConstants.DefaultStreamLifetimeMs) - DateTime.UtcNow,
            Timeout.InfiniteTimeSpan);

        streamTimers.TryAdd(stream.Id, timer);
    }

    private void ResetStreamTimer(MessageStream stream)
    {
        if (streamTimers.TryGetValue(stream.Id, out var existingTimer))
        {
            existingTimer.Change(
                stream.LastChunkReceivedAt.AddMilliseconds(TransportConstants.DefaultChunkTimeoutMs) - DateTime.UtcNow,
                Timeout.InfiniteTimeSpan);
        }
    }

    private void OnStreamTimeout(object? state)
    {
        if (state is Guid streamId)
        {
            logger?.Log($"Поток {streamId} превысил время жизни");
            RemoveStream(streamId);
        }
    }

    private void OnChunkAckRequested(Guid streamId, int chunkIndex, Guid connectionId, Guid roomId)
    {
        ChunkAckRequested?.Invoke(this, new ChunkAckRequestedEventArgs
        {
            StreamId = streamId,
            ChunkIndex = chunkIndex,
            ConnectionId = connectionId,
            RoomId = roomId
        });
    }

    private void RemoveStream(Guid streamId)
    {
        if (activeStreams.TryRemove(streamId, out var stream))
        {
            stream.Dispose();
        }

        if (streamTimers.TryRemove(streamId, out var timer))
        {
            timer.Dispose();
        }
    }

    private void OnMessageAssembled(Guid streamId, Guid connectionId, Guid roomId, byte[] data)
    {
        var args = new MessageAssembledEventArgs
        {
            StreamId = streamId,
            ConnectionId = connectionId,
            RoomId = roomId,
            Data = data
        };

        MessageAssembled?.Invoke(this, args);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        foreach (var stream in activeStreams.Values)
        {
            stream.Dispose();
        }
        activeStreams.Clear();

        foreach (var timer in streamTimers.Values)
        {
            timer.Dispose();
        }
        streamTimers.Clear();
    }
}
