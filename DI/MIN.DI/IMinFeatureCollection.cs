using MIN.Chat.DI;
using MIN.Core.DI;
using MIN.Discovery.DI;
using MIN.Helpers.DI;

namespace MIN.DI;

/// <summary>
/// Набор функциональностей для MIN
/// </summary>
public interface IMinFeatureCollection
{
    /// <inheritdoc cref="IChatFeatureCollection"/>
    IChatFeatureCollection Chat { get; }

    /// <inheritdoc cref="ICoreFeatureCollection"/>
    ICoreFeatureCollection Core { get; }

    /// <inheritdoc cref="IDiscoveryFeatureCollection"/>
    IDiscoveryFeatureCollection Discovery { get; }

    /// <inheritdoc cref="IHelperFeatureCollection"/>
    IHelperFeatureCollection Helper { get; }

    /// <summary>
    /// Версия приложения
    /// </summary>
    Version Version { get; }
}
