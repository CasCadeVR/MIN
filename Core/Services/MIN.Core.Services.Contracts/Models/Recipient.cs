using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.Core.Stores.Contracts.Registries.Models;

namespace MIN.Core.Services.Contracts.Models;

/// <summary>
/// Получатель сообщения
/// </summary>
public readonly struct Recipient
{
    /// <summary>
    /// Идентфикатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Идентфикатор участника
    /// </summary>
    public Guid? ParticipantId { get; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid? ConnectionId { get; }

    private Recipient(Guid roomId, Guid? participantId, Guid? connectionId)
    {
        (RoomId, ParticipantId, ConnectionId) = (roomId, participantId, connectionId);
    }

    /// <summary>
    /// Сформировать получателя в виде участника
    /// </summary>
    public static Recipient FromParticipant(Guid roomId, Guid? participantId)
        => new(roomId, participantId, null);

    /// <summary>
    /// Сформировать получателя в виде соединения
    /// </summary>
    public static Recipient FromConnection(Guid roomId, Guid? connectionId)
        => new(roomId, null, connectionId);

    /// <summary>
    /// Сформировать получателя в виде пустого получателя
    /// </summary>
    public static Recipient FromEmpty(Guid roomId)
        => new(roomId, null, null);

    /// <summary>
    /// Локальное ли соединения
    /// </summary>
    public bool IsLocal => ConnectionId == CoreRegistryConstants.LocalConnectionId;

    /// <summary>
    /// Пустой ли получатель
    /// </summary>
    public bool IsEmpty => !ParticipantId.HasValue && !ConnectionId.HasValue;

    /// <summary>
    /// Получить идентфикатор соединения от того что есть
    /// </summary>
    public Guid ResolveAsync(IParticipantConnectionRegistry registry)
    {
        if (ConnectionId.HasValue)
        {
            return ConnectionId.Value;
        }
        if (ParticipantId.HasValue)
        {
            return registry.GetConnectionIdFromParticipantId(RoomId, ParticipantId.Value);
        }

        throw new InvalidOperationException("Recipient is empty");
    }
}
