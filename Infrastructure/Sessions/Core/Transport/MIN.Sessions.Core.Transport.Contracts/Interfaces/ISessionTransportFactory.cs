namespace MIN.Sessions.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Фабрика по предоставлению междупроцессорного транспорта
/// </summary>
public interface ISessionTransportFactory
{
    /// <summary>
    /// Создать транспорт исходя от системы
    /// </summary>
    ISessionProcessTransport Create();

    /// <summary>
    /// Закрыть транспорт
    /// </summary>
    void Destroy(ISessionProcessTransport transport);
}
