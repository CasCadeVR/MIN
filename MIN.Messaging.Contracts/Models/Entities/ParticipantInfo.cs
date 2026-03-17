using MIN.Entities.Contracts;

namespace MIN.Messaging.Contracts.Models.Entities
{
    /// <summary>
    /// Данные участника для передачи по сети
    /// </summary>
    public record ParticipantInfo : IParticipantData
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; } = string.Empty;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ParticipantInfo"/>
        /// </summary>
        /// <param name="participant"></param>
        public ParticipantInfo(IParticipantData participant)
        {
            Id = participant.Id;
            Name = participant.Name;
        }
    }
}
