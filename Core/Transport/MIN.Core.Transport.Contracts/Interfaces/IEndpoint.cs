using MIN.Core.Transport.Contracts.Enum;

namespace MIN.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Базовый интерфейс для информации о точке подключения
/// </summary>
public interface IEndpoint
{
    /// <inheritdoc cref="TransportType"/>
    TransportType Type { get; }

    /// <inheritdoc cref="AddressOrigin"/>
    AddressOrigin Origin { get; }

    /// <summary>
    /// Получить строковое представление адреса
    /// </summary>
    string ToString();
}
