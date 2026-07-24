using System;
using Avalonia;
using Avalonia.Controls;
using MIN.Desktop.Contracts.Constants;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="Window"/>
/// </summary>
public static class WindowExtensions
{
    /// <summary>
    /// Получить контекст окна
    /// </summary>
    public static Window? GetWindow(this Visual visual) => TopLevel.GetTopLevel(visual) as Window;

    /// <summary>
    /// Установить стили из используемой платформы
    /// </summary>
    public static void ApplyPlatformWindowStyle(this Window window)
    {
        if (window == null)
        {
            return;
        }

        if (OperatingSystem.IsLinux())
        {
            // On Linux systems, Avalonia has trouble allowing windows to resize without "decorations". So we enable it in full, but hide the custom titlebar as it'll look bad.
            window.WindowDecorations = WindowDecorations.Full;
            DesignAttachments.SetUseCustomTitleBar(window, false);
        }
        else if (OperatingSystem.IsMacOS())
        {
            // On MacOS, it's uncommon to override the system titlebar
            window.WindowDecorations = WindowDecorations.Full;
            window.ExtendClientAreaToDecorationsHint = false;
            window.ExtendClientAreaTitleBarHeightHint = -1;
            DesignAttachments.SetUseCustomTitleBar(window, false);
        }
    }

    /// <summary>
    /// Закрыть окно как пользователь
    /// </summary>
    public static void CloseByUser(this Window? window, object? dialogResult = null)
    {
        if (window == null)
        {
            return;
        }
        window.Closed += WindowOnClosed;
        window.Close(dialogResult);

        static void WindowOnClosed(object? sender, EventArgs e)
        {
            if (sender is not Window window)
            {
                return;
            }
            window.Closed -= WindowOnClosed;
        }
    }

    /// <summary>
    /// Закрыть окно кодом
    /// </summary>
    public static void CloseByCode(this Window? window, object? dialogResult = null)
    {
        if (window == null)
        {
            return;
        }
        window.Close(dialogResult);
    }
}
