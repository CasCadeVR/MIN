using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Contracts.Models.ReferenceCommands;

namespace MIN.Desktop.Infrastructure.Services;

/// <summary>
/// Сервис уведомлений внутри приложения
/// </summary>
public static class InAppNotifier
{
    /// <summary>
    /// Вызвать уведомление ошибки
    /// </summary>
    public static void Error(string message)
    {
        WeakReferenceMessenger.Default.Send(new NotificationAddReferenceCommand(new NotificationItem(message, NotificationType.Error)));
    }

    /// <summary>
    /// Вызвать уведомление информации
    /// </summary>
    public static void Info(string message)
    {
        WeakReferenceMessenger.Default.Send(new NotificationAddReferenceCommand(new NotificationItem(message)));
    }

    /// <summary>
    /// Вызвать уведомление предупреждения
    /// </summary>
    public static void Warning(string message)
    {
        WeakReferenceMessenger.Default.Send(new NotificationAddReferenceCommand(new NotificationItem(message, NotificationType.Warning)));
    }

    /// <summary>
    /// Вызвать уведомление успеха
    /// </summary>
    public static void Success(string message)
    {
        WeakReferenceMessenger.Default.Send(new NotificationAddReferenceCommand(new NotificationItem(message, NotificationType.Success)));
    }

    /// <summary>
    /// Вызвать уведомление DEBUG
    /// </summary>
    [Conditional("DEBUG")]
    public static void Debug(string message, [CallerMemberName] string memberName = "")
    {
        WeakReferenceMessenger.Default.Send(new NotificationAddReferenceCommand(new NotificationItem($"Error in '{memberName}':{Environment.NewLine}{message}", NotificationType.Success)));
    }
}
