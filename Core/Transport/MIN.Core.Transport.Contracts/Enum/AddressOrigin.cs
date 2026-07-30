namespace MIN.Core.Transport.Contracts.Enum;

/// <summary>
/// Откуда был получен адрес
/// </summary>
public enum AddressOrigin
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
