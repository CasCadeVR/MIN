using MIN.Core.Messaging.Contracts;
using MIN.Discovery.Services.Contracts.Models;

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
    /// Информация об активных комнатах хоста
    /// </summary>
    public List<RoomDiscoveryInfo> RoomDiscoveryInfos { get; set; } = [];
}
