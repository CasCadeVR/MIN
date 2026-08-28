using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Events.Sending;
using MIN.Core.Streaming.Contracts.Models;

namespace MIN.Core.Streaming.Contracts.Interfaces;

/// <summary>
/// Менеджер отправки потоковых данных
/// </summary>
public interface IStreamManager
{
    /// <summary>
    /// Событие запроса на отправку ACK
    /// </summary>
    event Func<ChunkSendedEventArgs, CancellationToken, Task>? ChunkSended;

    /// <summary>
    /// Поток дошёл без проблем
    /// </summary>
    event Func<StreamCompletedEventArgs, CancellationToken, Task>? OnStreamCompleted;

    /// <summary>
    /// Поток провалился
    /// </summary>
    event Func<StreamFailedEventArgs, CancellationToken, Task>? OnStreamFailed;

    /// <summary>
    /// Отправляет данные через поток
    /// </summary>
    Task SendAsync(
        ReadOnlyMemory<byte> data,
        StreamOptions options,
        Guid roomId,
        Guid recipientConnectionId,
        Guid? serverConnectionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет данные через поток, читая из Stream порциями (не загружает всё в память)
    /// </summary>
    Task SendAsync(
        Stream source,
        StreamOptions options,
        Guid roomId,
        Guid recipientConnectionId,
        Guid? serverConnectionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обрабатывает входящие данные (проверяет ACK)
    /// </summary>
    void ProcessAck(byte[] data);
}
