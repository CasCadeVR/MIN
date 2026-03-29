namespace MIN.Discovery.Services.Contracts.Exceptions;

/// <summary>
/// Ошибка нахождения комнаты
/// </summary>
public class DiscoveryException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryException"/>
    /// </summary>
    public DiscoveryException(string message) : base(message) { }
}
