namespace MIN.Helpers.Contracts.Interfaces.SettingsServices;

/// <summary>
/// Сервис для хранения настроек
/// </summary>
public interface ISettingsStorage
{
    /// <summary>
    /// Загрузить настройки
    /// </summary>
    Models.Settings Load();

    /// <summary>
    /// Сохранить настройки
    /// </summary>
    void Save(Models.Settings settings);
}
