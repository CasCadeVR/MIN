namespace MIN.Transport.Contracts;

/// <summary>
/// Информация о текущем соединении
/// </summary>
public sealed class ConnectionInfo
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор участника (локального или удалённого)
    /// </summary>
    public Guid ParticipantId { get; init; }

    /// <summary>
    /// Точка подключения
    /// </summary>
    public IEndpoint Endpoint { get; init; } = null!;

    /// <summary>
    /// Флаг подключения
    /// </summary>
    public bool IsConnected { get; init; }
}
