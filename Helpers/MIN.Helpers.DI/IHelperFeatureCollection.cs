using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;

namespace MIN.Helpers.DI;

/// <summary>
/// Набор функциональностей для Helper
/// </summary>
public interface IHelperFeatureCollection
{
    /// <inheritdoc cref="ISettingsProvider"/>
    ISettingsProvider SettingsProvider { get; }

    /// <inheritdoc cref="ILocalNetworkComputerProvider"/>
    ILocalNetworkComputerProvider ComputerProvider { get; }

    /// <inheritdoc cref="INotificationService"/>
    INotificationService NotificationService { get; }

    /// <inheritdoc cref="ILoggerProvider"/>
    ILoggerProvider Logger { get; }

    /// <inheritdoc cref="IIdentityService"/>
    IIdentityService IdentityService { get; }
}
