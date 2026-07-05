using System;
using MIN.Core.Entities.Contracts.Models;
using MIN.Desktop.ViewModels.Pages;

namespace MIN.Desktop.Contracts.Interfaces;

/// <summary>
/// Менеджер страниц чатов
/// </summary>
public interface IChatViewManager
{
    /// <summary>
    /// Зарегистрировать панель чата
    /// </summary>
    void RegisterChat(RoomInfo roomInfo, ChatViewModel view);

    /// <summary>
    /// Удалить связь панели чата
    /// </summary>
    void UnregisterChat(Guid roomId);

    /// <summary>
    /// Получить панель чата
    /// </summary>
    ChatViewModel? GetChatView(Guid roomId);
}
