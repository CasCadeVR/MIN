namespace MIN.Sessions.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Фабрика по предоставлению междупроцессорного транспорта
/// </summary>
public interface ISessionTransportFactory
{
    /// <summary>
    /// Создать транспорт исходя от нужд программы
    /// </summary>
    ISessionProcessTransport Create(string? preferredTransport);

    /// <summary>
    /// Закрыть транспорт
    /// </summary>
    void Destroy(ISessionProcessTransport transport);
}
