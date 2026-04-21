namespace MIN.Desktop.Components.Controls.ContextMenuStrips;

/// <summary>
/// <see cref="ContextMenuStrip"/> для <see cref="ParticipantCard"/>
/// </summary>
public class ParticipantCardContextMenuStrip : ContextMenuStrip
{
    /// <summary>
    /// Событие по нажатию на <see cref="ParticipantCardContextMenuStrip"/>
    /// </summary>
    public Action? OnItemClick { get; set; }

    /// <summary>
    /// Иницилизирует новый экземпляр <see cref="ParticipantCardContextMenuStrip"/>
    /// </summary>
    public ParticipantCardContextMenuStrip()
    {
        var showPictureToolStripMenuItem = new ToolStripMenuItem()
        {
            Size = new Size(180, 22)
        };
        showPictureToolStripMenuItem.Click += PictureBoxContextMenuStrip_Click;
        Items.AddRange(new ToolStripItem[] { showPictureToolStripMenuItem });
    }

    private void PictureBoxContextMenuStrip_Click(object? sender, EventArgs e)
    {
        OnItemClick?.Invoke();
    }
}
