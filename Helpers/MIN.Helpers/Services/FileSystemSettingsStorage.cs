using System.Text.Json;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;

namespace MIN.Helpers.Services
{
    /// <inheritdoc cref="ISettingsStorage"/>
    public sealed class FileSystemSettingsStorage : ISettingsStorage
    {
        private readonly string settingsPath;
        private readonly JsonSerializerOptions jsonOptions;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="FileSystemSettingsStorage"/>
        /// </summary>
        public FileSystemSettingsStorage(IAppDataProvider appDataProvider)
        {
            var directory = Directory.CreateDirectory(Path.Combine(appDataProvider.BaseDirectory, "settings")).FullName;
            settingsPath = Path.Combine(directory, "settings.json");
            jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        Settings ISettingsStorage.Load()
        {
            try
            {
                if (!File.Exists(settingsPath))
                {
                    return new Settings();
                }
                var json = File.ReadAllText(settingsPath);
                return JsonSerializer.Deserialize<Settings>(json, jsonOptions) ?? new Settings();
            }
            catch
            {
                return new Settings();
            }
        }

        void ISettingsStorage.Save(Settings settings)
        {
            var json = JsonSerializer.Serialize(settings, jsonOptions);
            File.WriteAllText(settingsPath, json);
        }
    }

}
