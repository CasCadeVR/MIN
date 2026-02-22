using MIN.Services.Contracts.Models;

namespace MIN.Services.Extensions
{
    /// <summary>
    /// Расширения для <see cref="Participant"/>
    /// </summary>
    public static class ParticipantExtensions
    {
        /// <summary>
        /// Создаёт полную копию участника для сериализации
        /// </summary>
        public static Participant GetSerializableCopy(this Participant participant)
            => new()
            {
                Id = participant.Id,
                Name = participant.Name,
                PCName = participant.PCName,
            };
    }
}
