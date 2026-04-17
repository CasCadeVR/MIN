using MIN.Core.Headers.Contracts.Enums;

namespace MIN.Core.Streaming.Contracts.Models;

/// <summary>
/// Состояние собираемого потока
/// </summary>
public sealed class MessageStream : IDisposable
{
    private readonly MemoryStream buffer;
    private readonly HashSet<int> receivedIndices = [];
    private readonly object lockObj = new();
    private bool disposed;

    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Идентификатор соединения отправителя
    /// </summary>
    public Guid ConnectionId { get; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Нужно ли отправлять ACK
    /// </summary>
    public bool RequiresAcks { get; }

    /// <summary>
    /// Ожидаемое количество пакетов
    /// </summary>
    public int ExpectedChunks { get; }

    /// <summary>
    /// Время создания потока
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Время последнего полученного пакета
    /// </summary>
    public DateTime LastChunkReceivedAt { get; set; }

    /// <summary>
    /// Завершён ли поток
    /// </summary>
    public bool IsComplete { get; private set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageStream"/>
    /// </summary>
    public MessageStream(Guid streamId, Guid connectionId, Guid roomId, int expectedChunks, bool requiresAcks)
    {
        Id = streamId;
        ConnectionId = connectionId;
        RoomId = roomId;
        ExpectedChunks = expectedChunks;
        RequiresAcks = requiresAcks;
        CreatedAt = DateTime.UtcNow;
        LastChunkReceivedAt = CreatedAt;
        buffer = new MemoryStream();
    }

    /// <summary>
    /// Добавляет пакет в буфер
    /// </summary>
    public byte[]? AddChunk(StreamChunk chunk)
    {
        lock (lockObj)
        {
            if (disposed || IsComplete)
            {
                return null;
            }

            if (receivedIndices.Contains(chunk.Index))
            {
                return null;
            }

            receivedIndices.Add(chunk.Index);
            buffer.Position = buffer.Length;
            buffer.Write(chunk.Data.Span);
            LastChunkReceivedAt = DateTime.UtcNow;

            if (chunk.Flags.HasFlag(StreamChunkFlags.End))
            {
                if (chunk.Index != ExpectedChunks - 1)
                {
                    throw new InvalidOperationException(
                        $"Поток {Id} завершён с неверным индексом. Ожидался {ExpectedChunks - 1}, получен {chunk.Index}");
                }

                IsComplete = true;
                return buffer.ToArray();
            }

            return null;
        }
    }

    /// <summary>
    /// Проверяет, все ли пакеты получены
    /// </summary>
    public bool AreAllChunksReceived() => receivedIndices.Count == ExpectedChunks;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        buffer.Dispose();
    }
}
