using MIN.Chat.Services.Contracts.Interfaces;

namespace MIN.Chat.DI.FeatureCollection;

/// <inheritdoc cref="IChatFeatureCollection"/>
public class ChatFeatureCollection : IChatFeatureCollection
{
    /// <inheritdoc />
    public IChatService ChatService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFeatureCollection"/>
    /// </summary>
    public ChatFeatureCollection(IChatService chatService)
    {
        ChatService = chatService;
    }
}
