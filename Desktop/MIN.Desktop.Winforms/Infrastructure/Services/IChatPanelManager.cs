using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.Views.Panels.PanelViews.ChatPanel;

namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Менеджер панелей чата
/// </summary>
public interface IChatPanelManager
{
    /// <summary>
    /// Зарегистрировать панель чата
    /// </summary>
    void RegisterChat(RoomInfo roomInfo, ChatPanelView panel);

    /// <summary>
    /// Удалить связь панели чата
    /// </summary>
    void UnregisterChat(Guid roomId);
}
