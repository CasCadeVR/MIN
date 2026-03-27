using MIN.Messaging.Contracts;
using MIN.Entities.Contracts.Models;

namespace MIN.Messaging.Stateless.RoomRelated;

/// <summary>
/// Ответ на запрос обнаружения, содержащий информацию о комнате
/// </summary>
public sealed class DiscoveryResponseMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.DiscoveryResponse;

    /// <inheritdoc />
    public override bool RequiresEncryption => false;

    /// <summary>
    /// Информация об обнаруженной комнате
    /// </summary>
    public RoomInfo Room { get; set; } = null!;
}
