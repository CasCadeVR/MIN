using MIN.Services.Contracts.Models;

namespace MIN.Services.Contracts.Interfaces
{
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
}
