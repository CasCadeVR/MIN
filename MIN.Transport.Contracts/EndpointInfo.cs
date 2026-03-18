namespace MIN.Transport.Contracts;

/// <summary>
/// Базовый интерфейс для информации о точке подключения
/// </summary>
public interface IEndpoint
{
    /// <inheritdoc />
    TransportType Type { get; }
}
