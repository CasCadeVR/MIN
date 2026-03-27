using MIN.Transport.Contracts.Interfaces;

namespace MIN.Transport.Contracts.Models;

/// <summary>
/// Базовое соеднинение
/// </summary>
public abstract class BaseConnection : IConnection
{
    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Точка подключения
    /// </summary>
    public IEndpoint Endpoint { get; init; } = null!;

    /// <summary>
    /// Активно ли соединение
    /// </summary>
    public virtual bool IsConnected { get; }
}
