using MIN.Chat.Services.Contracts.Interfaces;

namespace MIN.Chat.DI;

/// <summary>
/// Набор функциональностей для Chat
/// </summary>
public interface IChatFeatureCollection
{
    /// <inheritdoc cref="IChatService"/>
    IChatService ChatService { get; }
}
