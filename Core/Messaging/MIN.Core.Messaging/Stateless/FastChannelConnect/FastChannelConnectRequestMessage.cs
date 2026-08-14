using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Messages;
using MIN.Core.Transport.Contracts.Enum;

namespace MIN.Core.Messaging.Stateless.FastChannelConnect;

/// <summary>
/// Сообщение - запрос на подключение к быстрому каналу
/// </summary>
public sealed class FastChannelConnectRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageTypeTag TypeTag => MessageTypeTag.FastChannelConnectRequest;

    /// <inheritdoc />
    public override bool IsPublic => false;

    /// <summary>
    /// Откуда подключился участник (нужно, ибо плохо будет если он подключиться к локальному Ip
    /// </summary>
    public AddressOrigin AddressOrigin { get; set; }
}
