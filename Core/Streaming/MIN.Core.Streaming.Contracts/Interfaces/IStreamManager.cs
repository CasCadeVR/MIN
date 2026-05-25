using MIN.Core.Streaming.Contracts.Models;

namespace MIN.Core.Streaming.Contracts.Interfaces;

/// <summary>
/// Менеджер отправки потоковых данных
/// </summary>
public interface IStreamManager
{
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
