using MIN.Chat.Services.Contracts.Interfaces;

namespace MIN.Chat.DI.FeatureCollection;

/// <inheritdoc cref="IChatFeatureCollection"/>
public class ChatFeatureCollection : IChatFeatureCollection
{
    /// <inheritdoc cref="IChatTextService"/>
    public IChatTextService ChatTextService { get; }

    /// <inheritdoc cref="IChatStatusService"/>
    public IChatStatusService ChatStatusService { get; }

    /// <inheritdoc cref="IChatFileService"/>
    public IChatFileService ChatFileService { get; }

    /// <inheritdoc cref="IChatSessionService"/>
    public IChatSessionService ChatSesssionService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFeatureCollection"/>
    /// </summary>
    public ChatFeatureCollection(IChatTextService chatTextService,
        IChatStatusService chatStatusService,
        IChatFileService chatFileService,
        IChatSessionService chatSessionService)
    {
        ChatTextService = chatTextService;
        ChatStatusService = chatStatusService;
        ChatFileService = chatFileService;
        ChatSesssionService = chatSessionService;
    }
}
