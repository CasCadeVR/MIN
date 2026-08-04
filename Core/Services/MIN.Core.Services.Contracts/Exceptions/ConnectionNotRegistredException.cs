namespace MIN.Core.Services.Contracts.Exceptions;

/// <summary>
/// Ошибка "Соединение не зарегистрировано"
/// </summary>
public class ConnectionNotRegistredException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionNotRegistredException"/>
    /// </summary>
    public ConnectionNotRegistredException(string message) : base(message) { }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionNotRegistredException"/>
    /// </summary>
    public ConnectionNotRegistredException(Guid connectionId) : base($"Соединение с id {connectionId} не зарегистрировано") { }
}
