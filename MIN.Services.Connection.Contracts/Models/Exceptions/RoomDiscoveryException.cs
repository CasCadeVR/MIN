namespace MIN.Services.Connection.Contracts.Models.Exceptions;

/// <summary>
/// Ошибка нахождения комнаты
/// </summary>
public class RoomDiscoveryException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomDiscoveryException"/>
    /// </summary>
    public RoomDiscoveryException(string message) : base(message) { }
}
