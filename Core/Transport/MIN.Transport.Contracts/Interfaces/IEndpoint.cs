using MIN.Transport.Contracts.Enum;

namespace MIN.Transport.Contracts.Interfaces;

/// <summary>
/// Базовый интерфейс для информации о точке подключения
/// </summary>
public interface IEndpoint
{
    /// <inheritdoc />
    TransportType Type { get; }
}
