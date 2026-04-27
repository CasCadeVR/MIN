using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
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

    /// <inheritdoc cref="IRoomStore"/>
    public IRoomStore RoomStore { get; }

    /// <inheritdoc cref="IRoomFactory"/>
    public IRoomFactory RoomFactory { get; }

    /// <inheritdoc cref="IEventBus"/>
    public IEventBus EventBus { get; }

    /// <inheritdoc cref="IMessageRouter"/>
    public IMessageRouter MessageRouter { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CoreFeatureCollection"/>
    /// </summary>
    public CoreFeatureCollection(IRoomConnector roomConnector,
        IRoomHoster roomHoster,
        IRoomStore roomStore,
        IRoomFactory roomFactory,
        IEventBus eventBus,
        IMessageRouter messageRouter)
    {
        RoomConnector = roomConnector;
        RoomHoster = roomHoster;
        RoomStore = roomStore;
        RoomFactory = roomFactory;
        EventBus = eventBus;
        MessageRouter = messageRouter;
    }
}
