namespace MIN.Transport.Contracts.Events;

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
    /// Идентификатор участника
    /// </summary>
    public Guid ParticipantId { get; }

    /// <summary>
    /// Флаг подключения (true – подключено, false – отключено)
    /// </summary>
    public bool IsConnected { get; }

    /// <summary>
    /// Сообщение об отключении
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionStateChangedEventArgs"/>
    /// </summary>
    public ConnectionStateChangedEventArgs(Guid roomId, Guid participantId, bool isConnected, string? reason = null)
    {
        RoomId = roomId;
        ParticipantId = participantId;
        IsConnected = isConnected;
        Reason = reason;
    }
}
