using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;

namespace MIN.Core.Messaging.Stateless;

/// <summary>
/// Подтверждение рукопожатия, содержащее публичный ключ сервера
/// </summary>
public sealed class HandshakeAckMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.HandshakeAck;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Подтверждение рукопожатия не шифруется
    /// </summary>
    public override bool RequiresEncryption => false;

    /// <summary>
    /// Хост комнаты
    /// </summary>
    public ParticipantInfo Participant { get; set; } = null!;

    /// <summary>
    /// Публичный ключ сервера/хоста.
    /// </summary>
    public byte[] PublicKey { get; set; } = null!;

    /// <summary>
    /// Инициализирует новый экземляр <see cref=""HandshakeAckMessage/>
    /// </summary>
    public HandshakeAckMessage() { }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="HandshakeAckMessage"/>
    /// </summary>
    /// <param name="handshakeMessage">Запрос на Handshake</param>
    public HandshakeAckMessage(HandshakeMessage handshakeMessage)
    {
        Participant = handshakeMessage.Participant;
        PublicKey = handshakeMessage.PublicKey;
    }
}
