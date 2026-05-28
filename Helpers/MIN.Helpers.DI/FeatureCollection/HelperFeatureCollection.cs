using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;

namespace MIN.Helpers.DI.FeatureCollection;

/// <inheritdoc cref="IHelperFeatureCollection"/>
public class HelperFeatureCollection : IHelperFeatureCollection
{
    /// <inheritdoc cref="ISettingsProvider"/>
    public ISettingsProvider SettingsProvider { get; }

    /// <inheritdoc cref="IAppDataProvider"/>
    public IAppDataProvider AppDataProvider { get; }

    /// <inheritdoc cref="INotificationService"/>
    public INotificationService NotificationService { get; }

    /// <inheritdoc cref="ILoggerProvider"/>
    public ILoggerProvider Logger { get; }

    /// <inheritdoc cref="IIdentityService"/>
    public IIdentityService IdentityService { get; }

    /// <inheritdoc cref="IVersionProvider"/>
    public IVersionProvider VersionProvider { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="HelperFeatureCollection"/>
    /// </summary>
    public HelperFeatureCollection(ISettingsProvider settingsProvider,
        IAppDataProvider appDataProvider,
        INotificationService notificationService,
        ILoggerProvider logger,
        IIdentityService identityService,
        IVersionProvider versionProvider)
    {
        SettingsProvider = settingsProvider;
        AppDataProvider = appDataProvider;
        NotificationService = notificationService;
        Logger = logger;
        IdentityService = identityService;
        VersionProvider = versionProvider;
    }
}
