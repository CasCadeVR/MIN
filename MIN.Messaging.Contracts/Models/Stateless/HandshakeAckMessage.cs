using MIN.Messaging.Contracts.Messages;

namespace MIN.Messaging.Contracts.Models.Stateless;

/// <summary>
/// Подтверждение рукопожатия, содержащее публичный ключ сервера
/// </summary>
public sealed class HandshakeAckMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.HandshakeAck;

    /// <summary>
    /// Подтверждение рукопожатия не шифруется
    /// </summary>
    public override bool RequiresEncryption => false;

    /// <summary>
    /// Публичный ключ сервера/хоста.
    /// </summary>
    public byte[] PublicKey { get; set; } = null!;
}
