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
    /// Инициализирует новый экземпляр <see cref="DiscoveryRawMessageReceivedEventArgs"/>
    /// </summary>
    public DiscoveryRawMessageReceivedEventArgs(byte[] data)
    {
        Data = data;
    }
}
