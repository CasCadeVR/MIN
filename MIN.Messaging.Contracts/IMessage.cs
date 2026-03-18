namespace MIN.Messaging.Contracts;

/// <summary>
/// Базовый интерфейс для всех сообщений, передаваемых по сети.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Уникальный идентификатор сообщения.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Тег типа сообщения для быстрой маршрутизации.
    /// </summary>
    MessageTypeTag TypeTag { get; }

    /// <summary>
    /// Флаг, указывающий, требуется ли шифрование для этого сообщения.
    /// </summary>
    bool RequiresEncryption { get; }

    /// <summary>
    /// Временная метка создания сообщения (UTC).
    /// </summary>
    DateTime Timestamp { get; }
}
