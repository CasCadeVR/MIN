using MIN.Desktop.Components.Controls.Buttons;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Contracts.Schemes;

namespace MIN.Desktop.Components.ComplexControls;

/// <summary>
/// Карточка, представляющая приложённый файл
/// </summary>
public class FileAttachmentCard : Label
{
    private Button? deleteButton;

    /// <summary>
    /// Приложение к файлу
    /// </summary>
    public FileAttachment FileAttachment { get; set; }

    /// <summary>
    /// Событие по нажатию кнопку удаления
    /// </summary>
    public event Action<FileAttachmentCard>? DeleteRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MultiFileAttachmentUploader"/>
    /// </summary>
    public FileAttachmentCard(FileAttachment fileAttachment)
    {
        Text = fileAttachment.FileName;
        TextAlign = ContentAlignment.MiddleCenter;
        FileAttachment = fileAttachment;
        BorderStyle = BorderStyle.FixedSingle;
        Cursor = Cursors.Hand;
        AddRemoveButton();
    }

    private void AddRemoveButton()
    {
        if (deleteButton != null || DesignMode)
        {
            return;
        }

        deleteButton = new InvertedButton
        {
            Text = "×",
            Size = new Size(24, 24),
            Padding = new Padding(0),
            Location = new Point(Width - 26, 2),
            Font = FontScheme.Caption,
            BackColor = Color.Red,
            ForeColor = Color.White,
        };

        deleteButton.FlatAppearance.BorderSize = 0;
        deleteButton.Click += (s, e) => DeleteRequested?.Invoke(this);

        Controls.Add(deleteButton);
        Controls.SetChildIndex(deleteButton, 0);
    }

    /// <inheritdoc cref="Control.OnSizeChanged(EventArgs)"/>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        deleteButton?.Location = new Point(Width - 26, 2);
    }
}
