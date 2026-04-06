using MIN.Core.Transport.Contracts.Enum;

namespace MIN.Core.Transport.Contracts.Interfaces
{
    /// <summary>
    /// Фабрика транспорта
    /// </summary>
    public interface ITransportFactory
    {
        /// <summary>
        /// Создать транспорт
        /// </summary>
        ITransport CreateTransport(TransportType type);
    }
}
