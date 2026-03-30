namespace MIN.Core.Transport.Contracts.Interfaces;

/// <summary>
/// Информация о текущем соединении
/// </summary>
public interface IConnection
{
    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Точка подключения
    /// </summary>
    IEndpoint Endpoint { get; init; }
}
