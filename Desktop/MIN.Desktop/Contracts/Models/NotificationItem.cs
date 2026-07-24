using System;
using System.Windows.Input;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Models.ReferenceCommands;

namespace MIN.Desktop.Contracts.Models;

/// <summary>
/// Модель уведомления внутри приложения
/// </summary>
public partial class NotificationItem : ObservableObject
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Тип уведомления
    /// </summary>
    public NotificationType Type { get; }

    /// <summary>
    /// Команда по нажатию на закрытие уведомления
    /// </summary>
    public ICommand CloseCommand { get; }

    /// <summary>
    /// Дата создания уведомления
    /// </summary>
    public DateTimeOffset Created { get; } = DateTimeOffset.Now;

    /// <summary>
    /// Было ли это уведомление закрыто
    /// </summary>
    [ObservableProperty]
    public partial bool IsDismissed { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NotificationItem"/>
    /// </summary>
    public NotificationItem(string message, NotificationType type = NotificationType.Information, ICommand? closeCommand = null)
    {
        Message = message;
        Type = type;
        CloseCommand = closeCommand ?? new RelayCommand(() => WeakReferenceMessenger.Default.Send(new NotificationCloseReferenceCommand(this)));
    }
}
