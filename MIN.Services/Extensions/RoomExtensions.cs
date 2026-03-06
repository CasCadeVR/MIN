using MIN.Services.Contracts.Models;

namespace MIN.Services.Extensions
{
    /// <summary>
    /// Расширения для <see cref="Room"/>
    /// </summary>
    public static class RoomExtensions
    {
        /// <summary>
        /// Создаёт полную копию комнаты для сериализации
        /// </summary>
        public static Room GetSerializableCopy(this Room room)
        {
            var copy = new Room(room.Name, room.MaximumParticipants)
            {
                Id = room.Id,
                HostParticipant = room.HostParticipant.GetSerializableCopy(),
            };

            var participants = room.CurrentParticipants;

            // Копируем всех текущих участников
            foreach (var participant in participants)
            {
                copy.AddParticipant(participant.GetSerializableCopy());
            }

            var chatHistory = room.ChatHistory;

            // Копируем все сообщения
            foreach (var message in chatHistory)
            {
                copy.AddMessage(message.GetSerializableCopy());
            }

            return copy;
        }
    }
}
