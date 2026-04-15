using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;

namespace MIN.Desktop.Infrastructure.Services
{
    ///<inheritdoc cref="ISettingsProvider"/>
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ISettingsStorage storage;
        private Settings? cachedSettings;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SettingsProvider"/>
        /// </summary>
        public SettingsProvider(ISettingsStorage storage)
        {
            this.storage = storage;
        }

        Settings ISettingsProvider.GetSettings()
        {
            return cachedSettings ??= storage.Load();
        }

        void ISettingsProvider.SaveSettings(Settings settings)
        {
            cachedSettings = settings;
            storage.Save(settings);
        }
    }
}
