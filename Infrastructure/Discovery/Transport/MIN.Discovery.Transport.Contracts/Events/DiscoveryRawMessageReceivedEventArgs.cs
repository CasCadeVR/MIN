using System;

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
    /// Идентификатор соединения (pipe) для маршрутизации ответа
    /// </summary>
    public Guid ConnectionId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryRawMessageReceivedEventArgs"/>
    /// </summary>
    public DiscoveryRawMessageReceivedEventArgs(byte[] data)
    {
        Data = data;
        ConnectionId = Guid.Empty;
    }

    /// <summary>
    /// Инициализирует новый экземпляр с указанием идентификатора соединения
    /// </summary>
    public DiscoveryRawMessageReceivedEventArgs(byte[] data, Guid connectionId)
    {
        Data = data;
        ConnectionId = connectionId;
    }
}
