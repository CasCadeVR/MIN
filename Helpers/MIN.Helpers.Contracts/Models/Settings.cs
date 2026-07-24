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
    /// Включена ли светлая тема
    /// </summary>
    public bool LightThemeEnabled { get; set; }

    /// <summary>
    /// Время ожидания поиска комнаты
    /// </summary>
    public int DiscoveryTimeout { get; set; } = 1500;

    /// <summary>
    /// Порт для обнаружения в сети
    /// </summary>
    public int DiscoveryPort { get; set; } = 42069;
}
