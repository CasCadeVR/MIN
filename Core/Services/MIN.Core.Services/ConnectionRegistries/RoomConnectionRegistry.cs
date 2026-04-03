using System.Collections.Concurrent;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;

namespace MIN.Core.Services.ConnectionRegistries
{
    /// <inheritdoc cref="IRoomConnectionRegistry"/>
    public sealed class RoomConnectionRegistry : IRoomConnectionRegistry
    {
        private readonly ConcurrentDictionary<Guid, Guid> roomIdByConnectionId = new();

        void IRoomConnectionRegistry.Associate(Guid connectionId, Guid roomId)
            => roomIdByConnectionId[connectionId] = roomId;

        Guid IRoomConnectionRegistry.GetRoomIdByConnectionId(Guid connectionId)
            => roomIdByConnectionId.TryGetValue(connectionId, out var roomId) ? roomId : throw new KeyNotFoundException();

        bool IRoomConnectionRegistry.TryGetRoomId(Guid connectionId, out Guid roomId)
            => roomIdByConnectionId.TryGetValue(connectionId, out roomId);

        void IRoomConnectionRegistry.Disassociate(Guid connectionId)
            => roomIdByConnectionId.TryRemove(connectionId, out _);
    }
}
