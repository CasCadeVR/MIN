namespace MIN.Core.Transport.Contracts.Enum;

/// <summary>
/// Откуда был получен IP
/// </summary>
public enum IpOrigin
{
    /// <summary>
    /// Из локальной сети
    /// </summary>
    LAN,

    /// <summary>
    /// Из VPN сервисов
    /// </summary>
    VPN,

    /// <summary>
    /// Из проброски порта как публичная сеть
    /// </summary>
    WAN
}
