namespace MIN.Transport.Contracts.Events;

/// <summary>
/// Аргументы события получения сырых данных от транспорта
/// </summary>
public sealed class RawMessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Полученные данные (байты)
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Идентификатор соединения отправителя сообщения
    /// </summary>
    public Guid ConnectionId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RawMessageReceivedEventArgs"/>
    /// </summary>
    public RawMessageReceivedEventArgs(byte[] data, Guid roomId, Guid сonnectionId)
    {
        Data = data;
        RoomId = roomId;
        ConnectionId = сonnectionId;
    }
}
