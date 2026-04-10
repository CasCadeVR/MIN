using MIN.Core.Services.Contracts.Constants;
using MIN.Core.Services.Contracts.Interfaces.ConnectionRegistries;

namespace MIN.Core.Services.Contracts.Models;

/// <summary>
/// Получатель сообщения
/// </summary>
public readonly struct Recipient
{
    /// <summary>
    /// Идентфикатор участника
    /// </summary>
    public Guid? ParticipantId { get; }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid? ConnectionId { get; }

    private Recipient(Guid? participantId, Guid? connectionId)
    {
        (ParticipantId, ConnectionId) = (participantId, connectionId);
    }

    /// <summary>
    /// Сформировать получателя в виде участника
    /// </summary>
    public static Recipient FromParticipant(Guid? participantId)
        => new(participantId, null);

    /// <summary>
    /// Сформировать получателя в виде соединения
    /// </summary>
    public static Recipient FromConnection(Guid? connectionId)
        => new(null, connectionId);

    /// <summary>
    /// Сформировать получателя в виде пустого получателя
    /// </summary>
    public static Recipient FromEmpty()
        => new(null, null);

    /// <summary>
    /// Локальный получатель (пользователь)
    /// </summary>
    public static Recipient Local => FromConnection(CoreServicesConstants.LocalConnectionId);

    /// <summary>
    /// Локальное ли соединения
    /// </summary>
    public bool IsLocal => ConnectionId == CoreServicesConstants.LocalConnectionId;

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
            return registry.GetConnectionIdFromParticipantId(ParticipantId.Value);
        }

        throw new InvalidOperationException("Recipient is empty");
    }
}
