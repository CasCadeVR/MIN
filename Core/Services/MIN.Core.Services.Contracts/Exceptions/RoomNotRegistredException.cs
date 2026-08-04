namespace MIN.Core.Services.Contracts.Exceptions;

/// <summary>
/// Ошибка "Комната не зарегистрирована"
/// </summary>
public class RoomNotRegistredException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomNotRegistredException"/>
    /// </summary>
    public RoomNotRegistredException(string message) : base(message) { }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomNotRegistredException"/>
    /// </summary>
    public RoomNotRegistredException(Guid roomId) : base($"Комната с id {roomId} не зарегистрирована") { }
}
