namespace MIN.Sessions.Core.Messaging.Contracts.Enums;

/// <summary>
/// Тип междупроцессорного сообщения
/// </summary>
public enum IpcMessageType : byte
{
    /// <summary>
    /// Первое сообщение после подключения
    /// </summary>
    Ready,

    /// <summary>
    /// Сообщение сессии
    /// </summary>
    InSession,

    /// <summary>
    /// Присоединения участнка
    /// </summary>
    ParticipantConnected,

    /// <summary>
    /// Отсоединения участника
    /// </summary>
    ParticipantDisconnected,

    /// <summary>
    /// Закрытие сервера
    /// </summary>
    ServerShutdown,
}
