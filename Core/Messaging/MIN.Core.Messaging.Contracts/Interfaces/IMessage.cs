namespace MIN.Core.Messaging.Contracts.Interfaces;

/// <summary>
/// Базовый интерфейс для всех сообщений, передаваемых по сети
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Уникальный идентификатор сообщения
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Тег типа сообщения для маршрутизации сообщения
    /// </summary>
    MessageTypeTag TypeTag { get; }

    /// <summary>
    /// Флаг, указывающий, требуется ли шифрование для этого сообщения
    /// </summary>
    bool RequiresEncryption { get; }

    /// <summary>
    /// Флаг, указывающий, требуется ли подтверждение получения пакета
    /// при передаче в потоке для этого сообщения
    /// </summary>
    bool RequireStreamAcks { get; }

    /// <summary>
    /// Флаг, указывающий, должно ли сообщение рассылаться всем
    /// </summary>
    bool IsPublic { get; }

    /// <summary>
    /// Временная метка создания сообщения
    /// </summary>
    DateTime Timestamp { get; }
}
