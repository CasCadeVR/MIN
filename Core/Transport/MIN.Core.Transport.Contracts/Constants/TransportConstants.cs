namespace MIN.Core.Transport.Contracts.Constants;

/// <summary>
/// Константы для транспорта
/// </summary>
public static class TransportConstants
{
    /// <summary>
    /// Максимальный размер буффера для чтения сообщения
    /// </summary>
    public const int MaximumBufferSize = 4 * 1024;

    /// <summary>
    /// Теоретически максимально возможный размер сервера
    /// </summary>
    public const int TheoraticallyPossibleMaximumRoomSize = 20;
}
