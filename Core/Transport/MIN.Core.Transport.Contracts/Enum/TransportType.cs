namespace MIN.Core.Transport.Contracts.Enum;

/// <summary>
/// Тип транспортного протокола
/// </summary>
public enum TransportType
{
    /// <summary>
    /// Именованные каналы
    /// </summary>
    NamedPipe,

    /// <summary>
    /// TCP
    /// </summary>
    Tcp,

    /// <summary>
    /// UDP
    /// </summary>
    Udp
}
