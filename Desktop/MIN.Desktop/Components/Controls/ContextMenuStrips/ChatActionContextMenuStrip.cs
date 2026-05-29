using MIN.Desktop.Components.Controls.ContextMenuStrips.ToolStripMenuItems;
using MIN.Desktop.Properties;

namespace MIN.Desktop.Components.Controls.ContextMenuStrips;

/// <summary>
/// <see cref="ContextMenuStrip"/> для чата
/// </summary>
public class ChatActionContextMenuStrip : ContextMenuStrip
{
    /// <summary>
    /// Событие по нажатию на <see cref="ChatActionContextMenuStrip"/>
    /// </summary>
    public Action? UploadFileClick { get; set; }

    /// <summary>
    /// Событие по нажатию на <see cref="ChatActionContextMenuStrip"/>
    /// </summary>
    public Action? StartSessionClick { get; set; }

    /// <summary>
    /// Иницилизирует новый экземпляр <see cref="ChatActionContextMenuStrip"/>
    /// </summary>
    public ChatActionContextMenuStrip()
    {
        var uploadFileToolStripMenuItem = new BaseToolStripMenuItem()
        {
            Text = "Отправить файл",
            Image = Resources.file,
        };
        uploadFileToolStripMenuItem.Click += UploadFileContextMenuStrip_Click;

        var startSessionToolStripMenuItem = new BaseToolStripMenuItem()
        {
            Text = "Использовать активность",
            Image = Resources.rocket,
        };
        startSessionToolStripMenuItem.Click += StartSessionContextMenuStrip_Click;

        Items.AddRange(new ToolStripItem[] { uploadFileToolStripMenuItem, startSessionToolStripMenuItem });
    }

    private void UploadFileContextMenuStrip_Click(object? sender, EventArgs e)
    {
        UploadFileClick?.Invoke();
    }

    private void StartSessionContextMenuStrip_Click(object? sender, EventArgs e)
    {
        StartSessionClick?.Invoke();
    }
}
