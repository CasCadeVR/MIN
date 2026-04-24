using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;

namespace MIN.Helpers.DI.FeatureCollection;

/// <inheritdoc cref="IHelperFeatureCollection"/>
public class HelperFeatureCollection : IHelperFeatureCollection
{
    /// <inheritdoc cref="ISettingsProvider"/>
    public ISettingsProvider SettingsProvider { get; }

    /// <inheritdoc cref="ILocalNetworkComputerProvider"/>
    public ILocalNetworkComputerProvider ComputerProvider { get; }

    /// <inheritdoc cref="INotificationService"/>
    public INotificationService NotificationService { get; }

    /// <inheritdoc cref="ILoggerProvider"/>
    public ILoggerProvider Logger { get; }

    /// <inheritdoc cref="IIdentityService"/>
    public IIdentityService IdentityService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="HelperFeatureCollection"/>
    /// </summary>
    public HelperFeatureCollection(ISettingsProvider settingsProvider,
        ILocalNetworkComputerProvider computerProvider,
        INotificationService notificationService,
        ILoggerProvider logger,
        IIdentityService identityService)
    {
        SettingsProvider = settingsProvider;
        ComputerProvider = computerProvider;
        NotificationService = notificationService;
        Logger = logger;
        IdentityService = identityService;
    }
}
