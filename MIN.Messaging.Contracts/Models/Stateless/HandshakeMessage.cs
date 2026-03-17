using MIN.Messaging.Contracts.Messages;
using MIN.Messaging.Contracts.Models.Entities;

namespace MIN.Messaging.Contracts.Models.Stateless;

/// <summary>
/// Сообщения для обмена криптографической информации
/// </summary>
public sealed class HandshakeMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.Handshake;

    /// <summary>
    /// Рукопожатие не шифруется (ключ ещё не установлен)
    /// </summary>
    public override bool RequiresEncryption => false;

    /// <summary>
    /// Информация об инициирующем участнике
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;

    /// <summary>
    /// Публичный ключ
    /// </summary>
    public byte[] PublicKey { get; set; } = null!;
}
