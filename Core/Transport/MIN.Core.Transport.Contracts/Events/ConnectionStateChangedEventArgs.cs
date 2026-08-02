using MIN.Core.Transport.Contracts.Enum;

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
    public DisconnectReason DisconnectReason { get; init; }

    /// <summary>
    /// Удалённая точка подключения
    /// </summary>
    public string? RemoteEndPoint { get; init; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionStateChangedEventArgs"/>
    /// </summary>
    public ConnectionStateChangedEventArgs(Guid сonnectionId, bool isConnected, DisconnectReason reason = DisconnectReason.None, Guid? serverConnectionId = null)
    {
        ConnectionId = сonnectionId;
        IsConnected = isConnected;
        DisconnectReason = reason;
        ServerConnectionId = serverConnectionId;
    }
}
