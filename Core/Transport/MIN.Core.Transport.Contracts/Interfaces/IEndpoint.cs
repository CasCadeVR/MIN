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
    /// Получить строковое представление места получения адреса
    /// </summary>
    string GetOrigin();

    /// <summary>
    /// Получить строковое представление места получения адреса
    /// </summary>
    string GetAddress();
}
