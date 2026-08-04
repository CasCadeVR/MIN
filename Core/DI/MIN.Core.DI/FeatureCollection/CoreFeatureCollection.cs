using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.DI.FeatureCollection;

/// <inheritdoc cref="ICoreFeatureCollection"/>
public class CoreFeatureCollection : ICoreFeatureCollection
{
    /// <inheritdoc cref="IRoomConnector"/>
    public IRoomConnector RoomConnector { get; }

    /// <inheritdoc cref="IRoomHoster"/>
    public IRoomHoster RoomHoster { get; }

    /// <inheritdoc cref="IRoomFactory"/>
    public IRoomFactory RoomFactory { get; }

    /// <inheritdoc cref="IRoomConnectionRegistry"/>
    public IRoomConnectionRegistry Registry { get; }

    /// <inheritdoc cref="IEventBus"/>
    public IEventBus EventBus { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CoreFeatureCollection"/>
    /// </summary>
    public CoreFeatureCollection(IRoomConnector roomConnector,
        IRoomHoster roomHoster,
        IRoomFactory roomFactory,
        IRoomConnectionRegistry registry,
        IEventBus eventBus)
    {
        RoomConnector = roomConnector;
        RoomHoster = roomHoster;
        RoomFactory = roomFactory;
        Registry = registry;
        EventBus = eventBus;
    }
}
