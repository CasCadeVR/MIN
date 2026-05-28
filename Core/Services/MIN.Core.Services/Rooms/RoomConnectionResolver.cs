using MIN.Core.Services.Contracts.Interfaces.Rooms;

namespace MIN.Core.Services.Rooms;

/// <inheritdoc cref="IRoomConnectionResolver"/>
public class RoomConnectionResolver : IRoomConnectionResolver
{
    private readonly IRoomHoster roomHoster;
    private readonly IRoomConnector roomConnector;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomConnectionResolver"/>
    /// </summary>
    public RoomConnectionResolver(IRoomHoster roomHoster,
        IRoomConnector roomConnector)
    {
        this.roomHoster = roomHoster;
        this.roomConnector = roomConnector;
    }

    Guid? IRoomConnectionResolver.GetServerConnectionIdByRoomId(Guid connectionId, Guid roomId)
    {
        Guid? serverConnectionId = null;

        if (roomHoster.IsHosting(roomId))
        {
            serverConnectionId = roomHoster.GetConnectionIdByRoomId(roomId);
        }

        return serverConnectionId;
    }

    Guid IRoomConnectionResolver.GetRoomIdByConnectionId(Guid connectionId, Guid? serverConnectionId)
    {
        Guid roomId;

        if (serverConnectionId != null)
        {
            roomId = roomHoster.GetRoomIdByConnectionId(serverConnectionId.Value);
        }
        else
        {
            roomId = roomConnector.GetRoomIdByConnectionId(connectionId);
        }

        return roomId;
    }
}
