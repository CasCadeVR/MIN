using MIN.Core.Entities.Contracts.Interfaces;

namespace MIN.Helpers.Contracts.Interfaces;

/// <summary>
/// Сервис по предоставлению данных о текущем пользователе
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Текущий пользователь приложения
    /// </summary>
    IParticipantData SelfPartcipant { get; }

    /// <summary>
    /// Установить данные пользователя
    /// </summary>
    void SetParticipant(IParticipantData participantData);

    /// <summary>
    /// Сбросить данные пользователя
    /// </summary>
    void ResetParticipant(IParticipantData participantData);
}
