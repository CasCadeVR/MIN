using System;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Modals;

namespace MIN.Desktop.ViewModels.Cards.Messages;

/// <summary>
/// Базовая view модель текстового сообщения, способное отредактироваться
/// </summary>
public abstract partial class BaseTextContentChatMessageViewModel : BaseChatMessageViewModel
{
    private readonly IDialogService dialogService;

    /// <summary>
    /// Идёт редактирование
    /// </summary>
    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    /// <summary>
    /// Сообщение уже отредактировано
    /// </summary>
    [ObservableProperty]
    public partial bool IsEdited { get; set; }

    /// <summary>
    /// Новый контент
    /// </summary>
    [ObservableProperty]
    public partial string EditContent { get; set; } = string.Empty;

    /// <summary>
    /// Текущий контент
    /// </summary>
    [ObservableProperty]
    public partial string Content { get; set; }

    /// <summary>
    /// Пользователь захотел отредактировать сообщение
    /// </summary>
    public Func<string, Task>? OnEditRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="BaseChatMessageViewModel"/>
    /// </summary>
    public BaseTextContentChatMessageViewModel(IMessage message,
        IContentEditable contentEditable,
        IDialogService dialogService,
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
        this.dialogService = dialogService;
        Content = contentEditable.Content;
        IsEdited = contentEditable.IsEdited;
    }

    /// <summary>
    /// Пришла новая версия сообщения
    /// </summary>
    public void MessageEdited(string newContent)
    {
        Content = newContent;
        IsEdited = true;
        IsEditing = false;
    }

    /// <summary>
    /// Сообщение отредактировано
    /// </summary>
    [RelayCommand]
    protected virtual async Task ConfirmEditMessage()
    {
        if (EditContent == string.Empty)
        {
            bool confirmation = await dialogService.ShowDialogAsync<DialogBoxViewModel>(model =>
            {
                model.Title = "Удаление сообщения";
                model.Description = $"Хотите удалить это сообщение?";
                model.ButtonOptions = ButtonOptions.YesNo;
            });

            if (confirmation)
            {
                OnDeleteRequested?.Invoke();
            }
            return;
        }

        EditContent = EditContent.Trim();

        if (Content != EditContent)
        {
            OnEditRequested?.Invoke(EditContent);
        }
        else
        {
            IsEditing = false;
        }
    }

    /// <summary>
    /// Отредактировать сообщение
    /// </summary>
    [RelayCommand]
    protected virtual void ToggleEditingMessage()
    {
        if (IsEditing)
        {
            CancelEditMessage();
            return;
        }

        EditContent = Content;
        IsEditing = true;
    }

    /// <summary>
    /// Остановить редактирование сообщения
    /// </summary>
    [RelayCommand]
    protected virtual void CancelEditMessage()
    {
        IsEditing = false;
    }
}
