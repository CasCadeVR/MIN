namespace MIN.Services.Contracts.Models.Events;

/// <summary>
/// Аргументы события выхода участника
/// </summary>
public class ParticipantLeftEventArgs : EventArgs
{
    /// <summary>
    /// Вышедший участник
    /// </summary>
    public Participant Participant { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantLeftEventArgs"/>
    /// </summary>
    public ParticipantLeftEventArgs(Participant participant) => Participant = participant;
}
