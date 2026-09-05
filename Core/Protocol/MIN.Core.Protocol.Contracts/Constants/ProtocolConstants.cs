using MIN.Core.Transport.Contracts.Helpers;

namespace MIN.Core.Protocol.Contracts.Constants;

/// <summary>
/// Константы для протокола
/// </summary>
public class ProtocolConstants
{
    /// <summary>
    /// Кодовое слово для любого подключения
    /// </summary>
    public const string ResponseStarter = ConnectionPreamble.Magic;

    /// <summary>
    /// Таймаут ожидания попытки подключения со стороны клиента
    /// </summary>
    public const int ClientSidePerTryTimeout = 3;

    /// <summary>
    /// Количество даваемых для клиента попыток перед тем как обозначить сервер как таймут
    /// </summary>
    public const int ClientSideRetryAmount = 5;

    /// <summary>
    /// Сколько дать миллисекунд хосту
    /// </summary>
    public const int ClientSideWarmupDelayMs = 20;

    /// <summary>
    /// Таймаут ожидания подключения со стороны хоста
    /// </summary>
    public const int ServerSideTimuout = 10;
}
