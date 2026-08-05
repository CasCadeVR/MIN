using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Lifecycle;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;

namespace MIN.Core.DI.FeatureCollection;

/// <summary>
/// Набор функциональностей для Core
/// </summary>
public interface ICoreFeatureCollection
{
    /// <inheritdoc cref="IRoomLifecycleManager"/>
    IRoomLifecycleManager Lifecycle { get; }

    /// <inheritdoc cref="IRoomFactory"/>
    IRoomFactory RoomFactory { get; }

    /// <inheritdoc cref="IRoomConnectionRegistry"/>
    IRoomConnectionRegistry Registry { get; }

    /// <inheritdoc cref="IEventBus"/>
    IEventBus EventBus { get; }

    /// <inheritdoc cref="IIdentityService"/>
    IIdentityService IdentityService { get; }
}
