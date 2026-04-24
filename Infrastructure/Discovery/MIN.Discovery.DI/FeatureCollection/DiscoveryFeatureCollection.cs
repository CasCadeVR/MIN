using MIN.Discovery.Services.Contracts.Interfaces;

namespace MIN.Discovery.DI.FeatureCollection;

/// <inheritdoc cref="IDiscoveryFeatureCollection"/>
public class DiscoveryFeatureCollection : IDiscoveryFeatureCollection
{
    /// <inheritdoc />
    public IDiscoveryService DiscoveryService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DiscoveryFeatureCollection"/>
    /// </summary>
    public DiscoveryFeatureCollection(IDiscoveryService discoveryService)
    {
        DiscoveryService = discoveryService;
    }
}
