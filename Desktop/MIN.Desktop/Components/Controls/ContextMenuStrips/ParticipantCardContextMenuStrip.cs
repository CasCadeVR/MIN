using MIN.Desktop.Components.Controls.ContextMenuStrips.ToolStripMenuItems;

namespace MIN.Desktop.Components.Controls.ContextMenuStrips;

/// <summary>
/// <see cref="ContextMenuStrip"/> для <see cref="ParticipantCard"/>
/// </summary>
public class ParticipantCardContextMenuStrip : ContextMenuStrip
{
    /// <summary>
    /// Событие по нажатию на <see cref="ParticipantCardContextMenuStrip"/>
    /// </summary>
    public Action? OnPrivateChatClick { get; set; }

    /// <summary>
    /// Событие по нажатию на <see cref="ParticipantCardContextMenuStrip"/>
    /// </summary>
    public Action? OnKickClick { get; set; }

    /// <summary>
    /// Иницилизирует новый экземпляр <see cref="ParticipantCardContextMenuStrip"/>
    /// </summary>
    public ParticipantCardContextMenuStrip()
    {
        var startPrivateChatToolStripMenuItem = new BaseToolStripMenuItem();
        startPrivateChatToolStripMenuItem.Click += StartPrivateChatContextMenuStrip_Click;

        var kickParticipantToolStripMenuItem = new BaseToolStripMenuItem();
        kickParticipantToolStripMenuItem.Text = "Кикнуть участника";
        kickParticipantToolStripMenuItem.Click += KickParticipantContextMenuStrip_Click;

        Items.AddRange(new ToolStripItem[] { startPrivateChatToolStripMenuItem, kickParticipantToolStripMenuItem });
    }

    private void StartPrivateChatContextMenuStrip_Click(object? sender, EventArgs e)
    {
        OnPrivateChatClick?.Invoke();
    }

    private void KickParticipantContextMenuStrip_Click(object? sender, EventArgs e)
    {
        OnKickClick?.Invoke();
    }
}
