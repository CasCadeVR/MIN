using MIN.Discovery.Services.Contracts.Interfaces;

namespace MIN.Discovery.DI.FeatureCollection;

/// <summary>
/// Набор функциональностей для Discovery
/// </summary>
public interface IDiscoveryFeatureCollection
{
    /// <inheritdoc cref="IDiscoveryService"/>
    IDiscoveryService DiscoveryService { get; }
}
