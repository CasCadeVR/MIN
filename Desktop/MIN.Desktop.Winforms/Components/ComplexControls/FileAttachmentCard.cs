using MIN.Desktop.Components.Controls.Buttons;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Winforms.Properties;

namespace MIN.Desktop.Components.ComplexControls;

/// <summary>
/// Карточка, представляющая приложённый файл
/// </summary>
public class FileAttachmentCard : Label
{
    private const int ButtonSize = 32;
    private const int ButtonCornerPadding = 2;
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
    /// Инициализирует новый экземпляр <see cref="FileAttachmentCard"/>
    /// </summary>
    public FileAttachmentCard(FileAttachment fileAttachment)
    {
        Text = fileAttachment.FileName;
        TextAlign = ContentAlignment.MiddleCenter;
        FileAttachment = fileAttachment;
        BorderStyle = BorderStyle.FixedSingle;

        AddRemoveButton();
    }

    private void AddRemoveButton()
    {
        if (deleteButton != null || DesignMode)
        {
            return;
        }

        deleteButton = new CommonButton
        {
            BackgroundImage = Resources.close,
            BackgroundImageLayout = ImageLayout.Zoom,
            Size = new Size(ButtonSize, ButtonSize),
            Margin = new Padding(0),
            Location = new Point(Width - ButtonSize + ButtonCornerPadding, ButtonCornerPadding),
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

        if (deleteButton != null)
        {
            deleteButton.Location = new Point(Width - ButtonSize + ButtonCornerPadding, ButtonCornerPadding);
        }
    }
}
