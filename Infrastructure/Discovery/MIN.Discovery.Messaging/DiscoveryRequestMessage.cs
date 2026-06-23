using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;

namespace MIN.Discovery.Messaging;

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
    /// Версия протокола локального обнаружения, которую поддерживает клиент
    /// </summary>
    public int DiscoveryProtocolVersion { get; set; } = 1;
}
