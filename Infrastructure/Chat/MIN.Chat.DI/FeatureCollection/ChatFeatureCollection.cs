using MIN.Chat.Services.Contracts.Interfaces;

namespace MIN.Chat.DI.FeatureCollection;

/// <inheritdoc cref="IChatFeatureCollection"/>
public class ChatFeatureCollection : IChatFeatureCollection
{
    /// <inheritdoc cref="IChatRoomService"/>
    public IChatRoomService ChatRoomService { get; }

    /// <inheritdoc cref="IChatTextService"/>
    public IChatTextService ChatTextService { get; }

    /// <inheritdoc cref="IChatStatusService"/>
    public IChatStatusService ChatStatusService { get; }

    /// <inheritdoc cref="IChatFileService"/>
    public IChatFileService ChatFileService { get; }

    /// <inheritdoc cref="IChatSessionService"/>
    public IChatSessionService ChatSessionService { get; }

    /// <inheritdoc cref="IChatVoiceService"/>
    public IChatVoiceService ChatVoiceService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFeatureCollection"/>
    /// </summary>
    public ChatFeatureCollection(IChatRoomService chatRoomService,
        IChatTextService chatTextService,
        IChatStatusService chatStatusService,
        IChatFileService chatFileService,
        IChatSessionService chatSessionService,
        IChatVoiceService chatVoiceService)
    {
        ChatRoomService = chatRoomService;
        ChatTextService = chatTextService;
        ChatStatusService = chatStatusService;
        ChatFileService = chatFileService;
        ChatSessionService = chatSessionService;
        ChatVoiceService = chatVoiceService;
    }
}
