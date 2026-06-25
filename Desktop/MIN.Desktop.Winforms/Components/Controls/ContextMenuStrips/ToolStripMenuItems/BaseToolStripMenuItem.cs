using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.Controls.ContextMenuStrips.ToolStripMenuItems;

/// <summary>
/// Базовый <see cref="ToolStripMenuItem"/>
/// </summary>
public class BaseToolStripMenuItem : ToolStripMenuItem
{
    /// <summary>
    /// Инциализирует новый экземпляр <see cref="BaseToolStripMenuItem"/>
    /// </summary>
    public BaseToolStripMenuItem()
    {
        ApplyBaseStyles();
    }

    /// <summary>
    /// Наложить базовые стили
    /// </summary>
    protected virtual void ApplyBaseStyles()
    {
        Font = FontScheme.Default;
        BackColor = ColorScheme.SecondaryAccent;
        ForeColor = ColorScheme.TextOnAccent;
        Size = new Size(200, 64);
    }
}
