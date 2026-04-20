using MIN.Desktop.Contracts;

namespace MIN.Desktop.Components.Controls.ContextMenuStrips;

/// <summary>
/// Базовый <see cref="ContextMenuStrip"/>
/// </summary>
public class BaseContextMenuStrip : ContextMenuStrip
{
    /// <summary>
    /// Иницилизирует новый экземпляр <see cref="BaseContextMenuStrip"/>
    /// </summary>
    public BaseContextMenuStrip()
    {
        ApplyBaseStyles();
    }

    /// <inheritdoc cref="BaseContextMenuStrip.ApplyBaseStyles"/>
    protected virtual void ApplyBaseStyles()
    {
        Size = new Size(181, 48);
        Font = FontScheme.Default;
        ForeColor = ColorScheme.TextPrimary;
        BackColor = ColorScheme.ChatAreaBackground;
    }
}
