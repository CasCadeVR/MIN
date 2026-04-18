using System.Collections.Concurrent;
using MIN.Core.Headers.Contracts.Constants;
using MIN.Core.Headers.Contracts.Enums;
using MIN.Core.Headers.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Constants;
using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Streaming.Services;

/// <inheritdoc cref="IChunkBufferAssembler"/>
public sealed class ChunkBufferAssembler : IChunkBufferAssembler, IDisposable
{
    private readonly ConcurrentDictionary<Guid, MessageStream> activeStreams = new();
    private readonly ConcurrentDictionary<Guid, Timer> streamTimers = new();
    private readonly ITransport transport;
    private readonly IHeaderManager headerManager;
    private readonly ILoggerProvider logger;
    private bool disposed;

    /// <inheritdoc />
    public event EventHandler<MessageAssembledEventArgs>? MessageAssembled;

    /// <inheritdoc />
    public event EventHandler<ChunkReceivedEventArgs>? ChunkReceived;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChunkBufferAssembler"/>
    /// </summary>
    public ChunkBufferAssembler(ITransport transport,
        IHeaderManager headerManager,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.headerManager = headerManager;
        this.logger = logger;
    }

    async Task IChunkBufferAssembler.ProcessStreamChunk(byte[] data, Guid connectionId, Guid roomId, CancellationToken cancellationToken)
    {
        if (disposed)
        {
            return;
        }

        var header = headerManager.ParseStreamChunkHeader(data);
        var chunkData = new ReadOnlyMemory<byte>(data, HeadersConstants.StreamHeaderSize,
            data.Length - HeadersConstants.StreamHeaderSize);

        var chunk = new StreamChunk
        {
            StreamId = header.StreamId,
            Flags = header.Flags,
            Index = header.Index,
            Total = header.Total,
            Data = chunkData
        };

        try
        {
            if (chunk.IsSingle)
            {
                if (chunk.Flags.HasFlag(StreamChunkFlags.RequiresAcks))
                {
                    await SendAck(chunk.StreamId, chunk.Index, connectionId, roomId, cancellationToken);
                }

                OnMessageAssembled(chunk.StreamId, connectionId, roomId, chunk.Data.ToArray());
                return;
            }

            var stream = activeStreams.GetOrAdd(chunk.StreamId, _ =>
                   CreateMessageStream(chunk, connectionId, roomId));

            if (stream.ConnectionId != connectionId)
            {
                return;
            }

            ResetStreamTimer(stream);

            if (chunk.Flags.HasFlag(StreamChunkFlags.Start))
            {
                stream.LastChunkReceivedAt = DateTime.UtcNow;
            }

            if (stream.RequiresAcks)
            {
                await SendAck(chunk.StreamId, chunk.Index, connectionId, roomId, cancellationToken);
            }

            ChunkReceived?.Invoke(this, new ChunkReceivedEventArgs
            {
                StreamId = stream.Id,
                ChunkIndex = chunk.Index,
                ConnectionId = connectionId,
                RoomId = roomId
            });

            var completeData = stream.AddChunk(chunk);
            if (completeData != null)
            {
                RemoveStream(chunk.StreamId);
                OnMessageAssembled(chunk.StreamId, connectionId, roomId, completeData);
            }
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при добавлении пакета: {ex.Message}");
            RemoveStream(chunk.StreamId);
            throw;
        }
    }

    private MessageStream CreateMessageStream(StreamChunk startChunk, Guid connectionId, Guid roomId)
    {
        var requiresAcks = startChunk.Flags.HasFlag(StreamChunkFlags.RequiresAcks);
        var stream = new MessageStream(startChunk.StreamId, connectionId, roomId, startChunk.Total, requiresAcks);
        StartStreamTimer(stream);
        return stream;
    }

    private void StartStreamTimer(MessageStream stream)
    {
        var timer = new Timer(
            OnStreamTimeout,
            stream.Id,
            stream.CreatedAt.AddMilliseconds(StreamingConstants.DefaultStreamLifetimeMs) - DateTime.UtcNow,
            Timeout.InfiniteTimeSpan);

        streamTimers.TryAdd(stream.Id, timer);
    }

    private void ResetStreamTimer(MessageStream stream)
    {
        if (streamTimers.TryGetValue(stream.Id, out var existingTimer))
        {
            existingTimer.Change(
                stream.LastChunkReceivedAt.AddMilliseconds(StreamingConstants.DefaultChunkTimeoutMs) - DateTime.UtcNow,
                Timeout.InfiniteTimeSpan);
        }
    }

    private void OnStreamTimeout(object? state)
    {
        if (state is Guid streamId)
        {
            logger.Log($"Поток {streamId} превысил время жизни");
            RemoveStream(streamId);
        }
    }

    private async Task SendAck(Guid streamId, int chunkIndex, Guid connectionId, Guid roomId, CancellationToken cancellationToken)
    {
        try
        {
            var ack = new byte[StreamingConstants.ChunkAckSize];
            ack[0] = (byte)HeaderMessageType.Ack;
            streamId.TryWriteBytes(new Span<byte>(ack, 1, 16));
            BitConverter.GetBytes(chunkIndex).CopyTo(ack, 17);

            await transport.SendAsync(ack, roomId, connectionId, cancellationToken);
            logger.Log($"Отправлен ACK для пакета {chunkIndex} потока {streamId}");
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при отправке ACK: {ex.Message}");
        }
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
