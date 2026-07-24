using MIN.Desktop.Contracts.Enums;

namespace MIN.Desktop.Contracts.Models.ReferenceCommands.Layout;

/// <summary>
/// Команда смены layout
/// </summary>
public sealed class LayoutModeChangedReferenceCommand(WindowLayout layout)
{
    /// <summary>
    /// Новый layout
    /// </summary>
    public WindowLayout Layout { get; } = layout;
}
