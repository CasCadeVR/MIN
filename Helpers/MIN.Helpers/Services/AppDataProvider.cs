using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Helpers.Services;

/// <inheritdoc cref="IAppDataProvider"/>
public sealed class AppDataProvider : IAppDataProvider
{
    /// <inheritdoc />
    public string BaseDirectory { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AppDataProvider"/>
    /// </summary>
    public AppDataProvider(Version version)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        BaseDirectory = Path.Combine(appData, "MIN", $"v.{version}");
    }
}
