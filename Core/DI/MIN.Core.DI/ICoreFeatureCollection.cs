using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Interfaces;

namespace MIN.Core.DI;

/// <summary>
/// Набор функциональностей для Core
/// </summary>
public interface ICoreFeatureCollection
{
    /// <inheritdoc cref="IRoomConnector"/>
    IRoomConnector RoomConnector { get; }

    /// <inheritdoc cref="IRoomHoster"/>
    IRoomHoster RoomHoster { get; }

    /// <inheritdoc cref="IRoomStore"/>
    IRoomStore RoomStore { get; }

    /// <inheritdoc cref="IRoomFactory"/>
    IRoomFactory RoomFactory { get; }

    /// <inheritdoc cref="IEventBus"/>
    IEventBus EventBus { get; }
}
