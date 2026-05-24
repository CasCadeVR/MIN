namespace MIN.Core.Transport.Contracts.Events;

/// <summary>
/// Аргументы события изменения состояния соединения
/// </summary>
public class ConnectionStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор соеднинения (к чему подключился)
    /// </summary>
    public Guid ConnectionId { get; init; }

    /// <summary>
    /// Идентификатор соеднинения (подключение комнаты)
    /// </summary>
    public Guid? ServerConnectionId { get; init; }

    /// <summary>
    /// Флаг подключения (true – подключено, false – отключено)
    /// </summary>
    public bool IsConnected { get; init; }

    /// <summary>
    /// Сообщение об отключении
    /// </summary>
    public string? LeavingMessage { get; init; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionStateChangedEventArgs"/>
    /// </summary>
    public ConnectionStateChangedEventArgs(Guid сonnectionId, bool isConnected, string? reason = null, Guid? serverConnectionId = null)
    {
        ConnectionId = сonnectionId;
        IsConnected = isConnected;
        LeavingMessage = reason;
        ServerConnectionId = serverConnectionId;
    }
}
