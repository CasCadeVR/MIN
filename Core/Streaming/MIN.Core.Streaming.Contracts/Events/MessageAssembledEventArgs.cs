namespace MIN.Core.Streaming.Contracts.Events;

/// <summary>
/// Событие успешной сборки сообщения из пакетов
/// </summary>
public sealed class MessageAssembledEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public required Guid StreamId { get; init; }

    /// <summary>
    /// Идентификатор соединения отправителя
    /// </summary>
    public required Guid ConnectionId { get; init; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public required Guid RoomId { get; init; }

    /// <summary>
    /// Собранные данные
    /// </summary>
    public required byte[] Data { get; init; }
}
