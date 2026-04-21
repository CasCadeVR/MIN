using MIN.Helpers.Contracts.Models.Enums;
using MIN.Core.Transport.Contracts.Enum;

namespace MIN.Helpers.Contracts.Models;

/// <summary>
/// Настройки
/// </summary>
public class Settings
{
    /// <summary>
    /// Имя своего участника по умолчанию
    /// </summary>
    public string DefaultParticipantName { get; set; } = string.Empty;

    /// <summary>
    /// Время ожидания поиска комнаты
    /// </summary>
    public int DiscoveryTimeout { get; set; } = 300;

    /// <summary>
    /// Метод поиска комнат
    /// </summary>
    public SearchMethod SearchMethod { get; set; } = SearchMethod.ClassRoom;

    /// <summary>
    /// Избранные компьютеры
    /// </summary>
    public IEnumerable<string> PreferredPCNames { get; set; } = [];

    /// <summary>
    /// Транспорт, который будет использоваться в локальной среде (NamedPipe, Tcp, ...)
    /// </summary>
    public TransportType TransportType { get; set; } = TransportType.NamedPipe;
}
