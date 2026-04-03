using System.Text.Json.Serialization;
using MIN.Core.Transport.Contracts.Enum;

namespace MIN.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Базовый интерфейс для информации о точке подключения
/// </summary>
public interface IEndpoint
{
    /// <inheritdoc />
    TransportType Type { get; }
}
