using MIN.Desktop.Contracts.Models;

namespace MIN.Desktop.Components.ComplexControls;

/// <summary>
/// Список приложенных файлов
/// </summary>
public partial class MultiFileAttachmentUploader : FlowLayoutPanel
{
    private readonly List<FileAttachment> attachments = [];

    private int CardSize => Height - Padding.Vertical;

    /// <summary>
    /// Событие, возникающее когда последний из файлов был удалён
    /// </summary>
    public Action? OnLastFileRemoved { get; set; } = null;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MultiFileAttachmentUploader"/>
    /// </summary>
    public MultiFileAttachmentUploader()
    {
        WrapContents = true;
        AutoScroll = true;
        FlowDirection = FlowDirection.LeftToRight;
        Padding = new Padding(5);
        BackColor = SystemColors.Control;
    }

    /// <summary>
    /// Прикреплённые файлы
    /// </summary>
    public IEnumerable<FileAttachment> AttachedFiles => attachments;

    /// <summary>
    /// Добавить файл
    /// </summary>
    public void AddFileAttachment(FileAttachment fileAttachment)
    {
        var pictureBox = CreateNewFileAttachment(fileAttachment);
        Controls.Add(pictureBox);
        attachments.Add(fileAttachment);
    }

    private FileAttachmentCard CreateNewFileAttachment(FileAttachment fileAttachment)
    {
        var card = new FileAttachmentCard(fileAttachment)
        {
            Size = new Size(CardSize, CardSize),
            Margin = new Padding(5)
        };

        card.DeleteRequested += OnFileAttachmentCardDeleteRequested;
        return card;
    }

    private void OnFileAttachmentCardDeleteRequested(FileAttachmentCard card)
    {
        attachments.RemoveAll(x => x == card.FileAttachment);
        Controls.Remove(card);
        card.Dispose();

        if (attachments.Count == 0)
        {
            OnLastFileRemoved?.Invoke();
        }
    }

    /// <summary>
    /// Загрузить несколько файлов
    /// </summary>
    public void UploadListOfAttachments(IEnumerable<FileAttachment> givenAttachments)
    {
        foreach (var attachment in givenAttachments)
        {
            AddFileAttachment(attachment);
        }
    }

    /// <summary>
    /// Очистить все приложения файлов
    /// </summary>
    public void Clear()
    {
        var pictureBoxes = Controls.OfType<FileAttachmentCard>().ToArray();

        foreach (var box in pictureBoxes)
        {
            OnFileAttachmentCardDeleteRequested(box);
        }

        attachments.Clear();
    }
}
