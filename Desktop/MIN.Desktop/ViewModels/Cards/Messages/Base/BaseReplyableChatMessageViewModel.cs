using System;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Desktop.ViewModels.Cards.Messages.Base;

/// <summary>
/// Базовая view модель любого сообщения
/// </summary>
public abstract partial class BaseReplyableChatMessageViewModel : BaseChatMessageViewModel
{
    /// <summary>
    /// Описание того сообщение, на которое это сообщение послужило ответом на него
    /// </summary>
    [ObservableProperty]
    public partial string? ReplyToDescription { get; set; }

    /// <summary>
    /// Сообщение выбрано в качестве создания на него ответа
    /// </summary>
    public Action? OnReplyRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseChatMessageViewModel"/>
    /// </summary>
    public BaseReplyableChatMessageViewModel() { }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseChatMessageViewModel"/>
    /// </summary>
    public BaseReplyableChatMessageViewModel(IMessage message,
       IReplyable? replyable,
       string name,
       Thickness timePadding,
       bool isLocal,
       bool isHost,
       bool removeHeaders)
        : base(message,
            name,
            timePadding,
            isLocal,
            isHost,
            removeHeaders)
    {
        ReplyToDescription = replyable?.ReplyToMessageDescription;
    }

    [RelayCommand]
    private void SetAsReply()
    {
        OnReplyRequested?.Invoke();
    }
}
