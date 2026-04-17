namespace MIN.Core.Transport.Contracts.Constants;

/// <summary>
/// Конфигурация для Transport
/// </summary>
public static class TransportConstants
{
    /// <summary>
    /// Максимально возможное количество соединений
    /// </summary>
    public const int RoomMaximumConnectionsAmount = 254;

    /// <summary>
    /// Размер буффера для сообщений
    /// </summary>
    public const int MessageBufferSize = 1024 * 4;
}
