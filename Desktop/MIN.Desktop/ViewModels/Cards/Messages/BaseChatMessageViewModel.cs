using System;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Cards.Messages;

/// <summary>
/// Базовая view модель любого сообщения
/// </summary>
public abstract partial class BaseChatMessageViewModel : CardViewModelBase
{
    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public Guid Id { get; }

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
    /// Margin между сообщениями по прошедшему времени
    /// </summary>
    public Thickness TimePadding { get; init; }

    /// <summary>
    /// Пользователь захотел удалить сообщение
    /// </summary>
    public Action? OnDeleteRequested;

    /// <summary>
    /// Пользователь захотел отредактировать сообщение
    /// </summary>
    public Action? OnEditRequested;

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
    public BaseChatMessageViewModel(Guid id,
       string name,
       DateTime time,
       Thickness timePadding,
       bool isLocal,
       bool isHost,
       bool removeHeaders,
       bool isPrivate)
    {
        Id = id;
        SenderName = name;
        Timestamp = time.ToShortTimeString();
        TimePadding = timePadding;
        IsLocal = isLocal;
        IsHost = isHost;
        RemoveHeaders = removeHeaders;
        IsPrivate = isPrivate;
    }

    /// <summary>
    /// Удалить сообщение
    /// </summary>
    [RelayCommand]
    protected virtual void DeleteMessage()
    {
        OnDeleteRequested?.Invoke();
    }

    /// <summary>
    /// Отредактировать сообщение
    /// </summary>
    [RelayCommand]
    protected virtual void EditMessage()
    {
        OnEditRequested?.Invoke();
    }
}
