using MIN.Chat.Services.Contracts.Interfaces;

namespace MIN.Chat.DI.FeatureCollection;

/// <summary>
/// Набор функциональностей для Chat
/// </summary>
public interface IChatFeatureCollection
{
    /// <inheritdoc cref="IChatRoomService"/>
    IChatRoomService ChatRoomService { get; }

    /// <inheritdoc cref="IChatTextService"/>
    IChatTextService ChatTextService { get; }

    /// <inheritdoc cref="IChatStatusService"/>
    IChatStatusService ChatStatusService { get; }

    /// <inheritdoc cref="IChatFileService"/>
    IChatFileService ChatFileService { get; }

    /// <inheritdoc cref="IChatSessionService"/>
    IChatSessionService ChatSessionService { get; }
}
