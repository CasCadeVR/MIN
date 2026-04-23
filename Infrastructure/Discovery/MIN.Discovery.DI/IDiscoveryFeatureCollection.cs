using MIN.Discovery.Services.Contracts.Interfaces;

namespace MIN.Discovery.DI;

/// <summary>
/// Набор функциональностей для Discovery
/// </summary>
public interface IDiscoveryFeatureCollection
{
    /// <inheritdoc cref="IDiscoveryService"/>
    IDiscoveryService DiscoveryService { get; }
}
