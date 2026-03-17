using MIN.Messaging.Contracts.Messages;

namespace MIN.Messaging.Contracts.Models.Stateless.RoomRelated;

/// <summary>
/// Запрос на обнаружение активных комнат в локальной сети
/// </summary>
public sealed class DiscoveryRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.DiscoveryRequest;

    /// <summary>
    /// Discovery запрос не шифруется
    /// </summary>
    public override bool RequiresEncryption => false;

    /// <summary>
    /// Версия протокола, которую поддерживает клиент
    /// </summary>
    public int ProtocolVersion { get; set; } = 1;
}
