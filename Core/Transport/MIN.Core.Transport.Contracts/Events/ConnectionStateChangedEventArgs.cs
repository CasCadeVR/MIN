namespace MIN.Core.Transport.Contracts.Events;

/// <summary>
/// Аргументы события изменения состояния соединения
/// </summary>
public sealed class ConnectionStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Идентификатор соеднинения (к чему подключился)
    /// </summary>
    public Guid ConnectionId { get; }

    /// <summary>
    /// Флаг подключения (true – подключено, false – отключено)
    /// </summary>
    public bool IsConnected { get; }

    /// <summary>
    /// Сообщение об отключении
    /// </summary>
    public string? LeavingMessage { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionStateChangedEventArgs"/>
    /// </summary>
    public ConnectionStateChangedEventArgs(Guid roomId, Guid сonnectionId, bool isConnected, string? reason = null)
    {
        RoomId = roomId;
        ConnectionId = сonnectionId;
        IsConnected = isConnected;
        LeavingMessage = reason;
    }
}
