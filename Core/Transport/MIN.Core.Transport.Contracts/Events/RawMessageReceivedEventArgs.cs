namespace MIN.Core.Transport.Contracts.Events;

/// <summary>
/// Аргументы события получения сырых данных от транспорта
/// </summary>
public class RawMessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Полученные данные (байты)
    /// </summary>
    public byte[] Data { get; init; }

    /// <summary>
    /// Идентификатор сервера соединения
    /// </summary>
    public Guid? ServerConnectionId { get; init; }

    /// <summary>
    /// Идентификатор соединения отправителя сообщения
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RawMessageReceivedEventArgs"/>
    /// </summary>
    public RawMessageReceivedEventArgs(byte[] data, Guid сonnectionId, Guid? serverConnectionId = null)
    {
        Data = data;
        ConnectionId = сonnectionId;
        ServerConnectionId = serverConnectionId;
    }
}
