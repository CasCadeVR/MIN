namespace MIN.Discovery.Transport.Contracts.Events;

/// <summary>
/// Аргументы события получения сырых данных от транспорта обнаружения
/// </summary>
public sealed class DiscoveryRawMessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Полученные данные (байты)
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Имя компьютера в сети, где было получено сообщение
    /// </summary>
    public string MachineName { get; }

    /// <summary>
    /// Идентификатор соединения (pipe) для маршрутизации ответа
    /// </summary>
    public Guid ConnectionId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryRawMessageReceivedEventArgs"/>
    /// </summary>
    public DiscoveryRawMessageReceivedEventArgs(byte[] data, string machineName)
    {
        Data = data;
        MachineName = machineName;
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryRawMessageReceivedEventArgs"/>
    /// </summary>
    public DiscoveryRawMessageReceivedEventArgs(byte[] data, string machineName, Guid connectionId)
    {
        Data = data;
        MachineName = machineName;
        ConnectionId = connectionId;
    }
}
