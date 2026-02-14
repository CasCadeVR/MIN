namespace MIN.Services.Contracts.Models.Events;

/// <summary>
/// Аргументы события входа участника
/// </summary>
public class ParticipantJoinedEventArgs : EventArgs
{
    /// <summary>
    /// Вошедший участник
    /// </summary>
    public Participant Participant { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantJoinedEventArgs"/>
    /// </summary>
    public ParticipantJoinedEventArgs(Participant participant) => Participant = participant;
}
