using MIN.Helpers.Contracts.Models;

namespace MIN.Helpers.Contracts.Interfaces.SettingsServices;

/// <summary>
/// Сервис по работе с настройками
/// </summary>
public interface ISettingsProvider
{
    /// <summary>
    /// Получить настройки
    /// </summary>
    Settings GetSettings();

    /// <summary>
    /// Сохранить настройки
    /// </summary>
    void SaveSettings(Settings settings);
}
