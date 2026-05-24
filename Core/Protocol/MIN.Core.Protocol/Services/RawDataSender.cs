using MIN.Core.Protocol.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Protocol.Services;

/// <inheritdoc cref="IRawDataSender"/>
public class RawDataSender : IRawDataSender
{
    private readonly ITransport transport;
    private readonly IRoomHoster roomHoster;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RawDataSender"/>
    /// </summary>
    public RawDataSender(ITransport transport, IRoomHoster roomHoster)
    {
        this.transport = transport;
        this.roomHoster = roomHoster;
    }

    async Task IRawDataSender.SendAsync(byte[] data, Guid roomId, Guid connectionId, CancellationToken cancellationToken)
    {
        Guid? serverRoomId = null;

        if (roomHoster.IsHosting(roomId))
        {
            serverRoomId = roomHoster.GetConnectionIdByRoomId(roomId);
        }

        await transport.SendAsync(data, connectionId, serverRoomId, cancellationToken);
    }
}
