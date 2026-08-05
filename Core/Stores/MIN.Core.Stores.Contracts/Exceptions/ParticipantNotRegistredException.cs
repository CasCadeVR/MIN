namespace MIN.Core.Stores.Contracts.Exceptions;

/// <summary>
/// Ошибка "Участник не зарегистрирован"
/// </summary>
public class ParticipantNotRegistredException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantNotRegistredException"/>
    /// </summary>
    public ParticipantNotRegistredException(string message) : base(message) { }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ParticipantNotRegistredException"/>
    /// </summary>
    public ParticipantNotRegistredException(Guid connectionId) : base($"Участник с id {connectionId} не зарегистрирован") { }
}
