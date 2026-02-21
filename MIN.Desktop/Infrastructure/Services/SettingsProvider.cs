using System.Text.Json;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Models;

namespace MIN.Desktop.Infrastructure.Services
{
    ///<inheritdoc cref="ISettingsProvider"/>
    public class SettingsProvider : ISettingsProvider
    {
        private static readonly string SettingsFilePath = Path.Combine(Application.StartupPath, "appsettings.json");

        private Settings settings;

        Settings ISettingsProvider.GetSettings()
        {
            Load();
            return settings;
        }

        void ISettingsProvider.SaveSettings(Settings settings)
        {
            this.settings = settings;
            Save();
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    settings = new Settings();

                var json = File.ReadAllText(SettingsFilePath);
                settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            catch
            {
                settings = new Settings();
            }
        }

        private void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
