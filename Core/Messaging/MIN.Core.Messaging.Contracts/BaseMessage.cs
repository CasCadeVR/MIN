using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Messaging.Contracts;

/// <summary>
/// Базовый класс для всех сообщений, передаваемых по сети
/// Предоставляет общие свойства Id и Timestamp
/// </summary>
public abstract class BaseMessage : IMessage
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public abstract MessageTypeTag TypeTag { get; }

    /// <inheritdoc />
    public virtual bool IsPublic { get; } = true;

    /// <inheritdoc />
    public virtual bool RequiresEncryption { get; } = true;
}
