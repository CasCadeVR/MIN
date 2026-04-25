using MIN.Desktop.Views.Panels.PanelViews;

namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Менеджер панелей чата
/// </summary>
public interface IChatPanelManager
{
    /// <summary>
    /// Зарегистрировать панель чата
    /// </summary>
    void RegisterChat(Guid roomId, ChatPanelView panel);

    /// <summary>
    /// Удалить связь панели чата
    /// </summary>
    void UnregisterChat(Guid roomId);

    /// <summary>
    /// Получить панель чата
    /// </summary>
    ChatPanelView? GetChatPanel(Guid roomId);
}
