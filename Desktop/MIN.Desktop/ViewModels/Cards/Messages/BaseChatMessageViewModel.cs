using System;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Cards.Messages;

/// <summary>
/// Базовая view модель любого сообщения
/// </summary>
public abstract partial class BaseChatMessageViewModel : CardViewModelBase
{
    /// <summary>
    /// Имя отправителя сообщения
    /// </summary>
    public string SenderName { get; init; } = string.Empty;

    /// <summary>
    /// Время отправления
    /// </summary>
    public string Timestamp { get; init; }

    /// <summary>
    /// Отправитель = хост
    /// </summary>
    public bool IsHost { get; init; }

    /// <summary>
    /// Отправитель = ты
    /// </summary>
    public bool IsLocal { get; init; }

    /// <summary>
    /// Нужно ли убрать заголовки
    /// </summary>
    /// <remarks>
    /// Например если сообщение повторяется одним и тем же участником
    /// </remarks>
    public bool RemoveHeaders { get; set; }

    /// <summary>
    /// Приватное ли сообщение
    /// </summary>
    public bool IsPrivate { get; init; }

    /// <summary>
    /// Выбрано ли сообщение
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseChatMessageViewModel"/>
    /// </summary>
    public BaseChatMessageViewModel()
    {
        Timestamp = DateTime.Now.ToShortTimeString();
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseChatMessageViewModel"/>
    /// </summary>
    public BaseChatMessageViewModel(string name,
       DateTime time,
       bool isLocal,
       bool isHost,
       bool removeHeaders)
    {
        IsHost = isHost;
        IsLocal = isLocal;
        RemoveHeaders = removeHeaders;
        SenderName = name;
        Timestamp = time.ToShortTimeString();
    }
}
