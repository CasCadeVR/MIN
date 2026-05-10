namespace MIN.Desktop.Components.Controls.ContextMenuStrips;

/// <summary>
/// <see cref="ContextMenuStrip"/> для <see cref="ChatFileMessageCard"/>
/// </summary>
public class FileMessageContextMenuStrip : ContextMenuStrip
{
    /// <summary>
    /// Событие по нажатию на <see cref="FileMessageContextMenuStrip"/>
    /// </summary>
    public Action? OnItemClick { get; set; }

    /// <summary>
    /// Иницилизирует новый экземпляр <see cref="FileMessageContextMenuStrip"/>
    /// </summary>
    public FileMessageContextMenuStrip()
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
