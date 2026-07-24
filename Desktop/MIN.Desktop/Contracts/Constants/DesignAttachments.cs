using Avalonia;
using Avalonia.Controls;

namespace MIN.Desktop.Contracts.Constants;

/// <summary>
/// Дополнения логики и свойств для дизайна элементов
/// </summary>
public class DesignAttachments : AvaloniaObject
{
    /// <summary>
    /// Использовать ли кастомный Titlebar или оставить по умолчанию
    /// </summary>
    public readonly static AttachedProperty<bool> UseCustomTitleBarProperty
        = AvaloniaProperty.RegisterAttached<DesignAttachments, Window, bool>("UseCustomTitleBar", true);

    /// <summary>
    /// Получить свойство "<inheritdoc cref="UseCustomTitleBarProperty"/>"
    /// </summary>
    public static bool GetUseCustomTitleBar(Window window) => window.GetValue(UseCustomTitleBarProperty);

    /// <summary>
    /// Установить свойство "<inheritdoc cref="UseCustomTitleBarProperty"/>"
    /// </summary>
    public static void SetUseCustomTitleBar(Window window, bool value) => window.SetValue(UseCustomTitleBarProperty, value);
}
