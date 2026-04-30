using MIN.Chat.DI.FeatureCollection;
using MIN.Core.DI.FeatureCollection;
using MIN.Discovery.DI.FeatureCollection;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.Helpers.DI.FeatureCollection;

namespace MIN.DI.FeatureCollection;

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

    /// <inheritdoc cref="IFileTransferFeatureCollection"/>
    IFileTransferFeatureCollection FileTransfer { get; }

    /// <inheritdoc cref="IHelperFeatureCollection"/>
    IHelperFeatureCollection Helper { get; }

    /// <summary>
    /// Версия приложения
    /// </summary>
    Version Version { get; }
}
