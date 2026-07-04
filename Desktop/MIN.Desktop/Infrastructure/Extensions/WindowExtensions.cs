using System;
using System.Collections.Generic;
using Avalonia.Controls;
using MIN.Desktop.Contracts.Constants;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="Window"/>
/// </summary>
public static class WindowExtensions
{
    private readonly static Dictionary<Window, bool> isClosingByUser = [];

    /// <summary>
    /// Установить стили из используемой платформы
    /// </summary>
    public static void ApplyPlatformWindowStyle(this Window window)
    {
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
        else if (OperatingSystem.IsWindows())
        {
            var windowHandle = window.TryGetPlatformHandle()?.Handle;
            if (!windowHandle.HasValue)
            {
                return;
            }
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
        isClosingByUser[window] = true;
        window.Close(dialogResult);

        static void WindowOnClosed(object? sender, EventArgs e)
        {
            if (sender is not Window window)
            {
                return;
            }
            window.Closed -= WindowOnClosed;
            isClosingByUser.Remove(window);
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
        isClosingByUser[window] = false;
        window.Close(dialogResult);
    }

    /// <summary>
    /// Закрывается ли окно пользователем
    /// </summary>
    public static bool IsClosingByUser(this Window? closingWindow, WindowClosingEventArgs? closingArgs = null)
    {
        if (closingWindow is not null && isClosingByUser.TryGetValue(closingWindow, out bool isByUser))
        {
            return isByUser;
        }
        if (closingArgs is { IsProgrammatic: false })
        {
            return true;
        }
        return false;
    }
}
