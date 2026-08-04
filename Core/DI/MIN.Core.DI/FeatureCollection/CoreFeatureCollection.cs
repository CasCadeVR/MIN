using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Interfaces;

namespace MIN.Core.DI.FeatureCollection;

/// <inheritdoc cref="ICoreFeatureCollection"/>
public class CoreFeatureCollection : ICoreFeatureCollection
{
    /// <inheritdoc cref="IRoomLifecycleManager"/>
    public IRoomLifecycleManager Lifecycle { get; }

    /// <inheritdoc cref="IRoomFactory"/>
    public IRoomFactory RoomFactory { get; }

    /// <inheritdoc cref="IRoomConnectionRegistry"/>
    public IRoomConnectionRegistry Registry { get; }

    /// <inheritdoc cref="IEventBus"/>
    public IEventBus EventBus { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CoreFeatureCollection"/>
    /// </summary>
    public CoreFeatureCollection(IRoomLifecycleManager lifecycle,
        IRoomFactory roomFactory,
        IRoomConnectionRegistry registry,
        IEventBus eventBus)
    {
        Lifecycle = lifecycle;
        RoomFactory = roomFactory;
        Registry = registry;
        EventBus = eventBus;
    }
}
