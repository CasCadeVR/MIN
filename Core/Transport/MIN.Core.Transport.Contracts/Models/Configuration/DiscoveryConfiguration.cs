namespace MIN.Core.Transport.Contracts.Models.Configuration;

/// <summary>
/// Конфигурация для Transport
/// </summary>
public sealed class TransportConfiguration
{
    /// <summary>
    /// Размер буффера для сообщений
    /// </summary>
    public int MessageBufferSize { get; set; }

    /// <summary>
    /// Максимально возможное количество соединений
    /// </summary>
    public int RoomMaximumConnectionsAmount { get; set; }
}
