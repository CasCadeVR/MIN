using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Services
{
    /// <inheritdoc cref="IRoomService"/>
    public class RoomService : IRoomService
    {
        private readonly List<Room> rooms = [];

        async Task<IEnumerable<Room>> IRoomService.GetAll(CancellationToken cancellationToken)
            => rooms;

        async Task<Room> IRoomService.Create(Room room, CancellationToken cancellationToken)
        {
            rooms.Add(room);
            return room;
        }

        async Task<Room> IRoomService.Update(Room room, CancellationToken cancellationToken)
        {
            var existing = rooms.Where(x => x.Id == room.Id).FirstOrDefault()
                ?? throw new NullReferenceException($"Не нашлось комнаты с Id {room.Id}");

            existing.Name = room.Name;
            existing.MaximumParticipants = room.MaximumParticipants;
            existing.HostParticipant = room.HostParticipant;

            return room;
        }

        async Task IRoomService.Delete(Room room, CancellationToken cancellationToken)
        {
            rooms.Remove(room);
        }
    }
}
