using System;
using Avalonia.Controls;
using MIN.Desktop.Contracts.Constants;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="Window"/>
/// </summary>
public static class WindowExtensions
{
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
}
