using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Discovery.Messaging;

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

    /// <summary>
    /// Точка подключения к комнате
    /// </summary>
    public IEndpoint Endpoint { get; set; } = null!;
}
