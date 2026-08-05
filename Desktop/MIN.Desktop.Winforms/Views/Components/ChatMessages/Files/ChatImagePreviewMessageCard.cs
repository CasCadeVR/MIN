using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Desktop.Components.Controls.ContextMenuStrips;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Components.ChatMessages;
using MIN.Desktop.Winforms.Properties;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения, представляющая картинку от пользователя
/// </summary>
public partial class ChatImagePreviewMessageCard : BaseChatMessageCard, IDisposable, IResizableComponent
{
    private readonly IFileTransferFeatureCollection fileTransferFeatureCollection = null!;
    private readonly IEventBus eventBus = null!;
    private readonly FileMetadataMessage fileMetadataMessage = null!;
    private readonly SynchronizationContext uiContext = null!;
    private readonly ParticipantInfo localParticipant = null!;
    private readonly string cachedFileFormat = string.Empty;

    private Size? cachedImgSize;
    private HashSet<IDisposable> eventTokens = null!;
    private bool isDownloading;
    private bool downloaded;
    private IDisposable fileTransferProgressSubsciptionToken = null!;

    /// <summary>
    /// Событие, возникающее по нажатию на кнопку скачать
    /// </summary>
    public event Func<Task>? OnDownloadRequested;

    /// <summary>
    /// Событие, возникающее по нажатию на кнопку отмены загрузки
    /// </summary>
    public event Func<Task>? OnCancelRequested;

    /// <summary>
    /// Событие по нажатию на контекстное меню карточки
    /// </summary>
    public Action? OnCardContextMenuStripClicked { get; set; }

    /// <inheritdoc />
    public Action? AskParentForResize { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatImagePreviewMessageCard"/>
    /// </summary>
    public ChatImagePreviewMessageCard() : base()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatImagePreviewMessageCard"/>
    /// </summary>
    public ChatImagePreviewMessageCard(IFileTransferFeatureCollection fileTransferFeatureCollection,
        IEventBus eventBus,
        FileMetadataMessage fileMetadataMessage,
        ParticipantInfo localParticipant,
        bool isHostMessage,
        bool removeHeaders)
        : base(fileMetadataMessage.Sender.Name,
            fileMetadataMessage.Timestamp,
            localParticipant.Id == fileMetadataMessage.SenderId,
            isHostMessage,
            removeHeaders)
    {
        InitializeComponent();
        this.fileTransferFeatureCollection = fileTransferFeatureCollection;
        this.eventBus = eventBus;
        this.fileMetadataMessage = fileMetadataMessage;
        this.localParticipant = localParticipant;

        cachedFileFormat = fileTransferFeatureCollection.FileHelperService
            .GetFileType(fileMetadataMessage.FileName);
        downloaded = !string.IsNullOrEmpty(fileMetadataMessage.FilePath) || fileMetadataMessage.AsDownloaded;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        FillLabels();
        ApplyStylings();
        UpdateIconOutOfState();
        PerformLayout();
        InitializeContextMenu();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        eventTokens =
        [
            eventBus.Subscribe<FileTransferStartedEvent>(OnFileTransferStarted),
            eventBus.Subscribe<FileTransferFailedEvent>(OnFileTransferFailed),
            eventBus.Subscribe<FileTransferCompletedEvent>(OnFileTransferCompleted)
        ];
    }

    private void InitializeContextMenu()
    {
        var pictureBoxContextMenuStrip = new FileMessageContextMenuStrip();
        pictureBoxContextMenuStrip.OnItemClick += () => OnCardContextMenuStripClicked?.Invoke();
        pictureBoxContextMenuStrip.Items[0].Text = "Показать в проводнике";
        ContextMenuStrip = pictureBoxContextMenuStrip;
    }

    private async Task OnFileTransferStarted(FileTransferStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.FileMetadataId != fileMetadataMessage.Id)
        {
            return;
        }

        isDownloading = true;

        fileTransferProgressSubsciptionToken = eventBus.Subscribe((FileTransferProgressEvent e, CancellationToken _) =>
        {
            if (eventMessage.FileMetadataId != fileMetadataMessage.Id)
            {
                return Task.CompletedTask;
            }

            var progress = 100 * e.BytesReceived / fileMetadataMessage.FileSize;
            uiContext.Post(_ =>
            {
                downloadProgressBar.Value = (int)Math.Min(progress, 100);
                fileNameAndSize.Text = $"{fileMetadataMessage.FileName} " +
                    $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(e.BytesReceived)}" +
                    $" / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
            }, this);

            return Task.CompletedTask;
        });

        uiContext.Post(_ =>
        {
            UpdateIconOutOfState();
            fileNameAndSize.Text = $"{fileMetadataMessage.FileName} 0 / " +
                $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
            splitContainerDownload.Panel2Collapsed = false;
        }, this);

        await Task.CompletedTask;
    }

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.FileMetadataId != fileMetadataMessage.Id
            || eventMessage.SenderId != localParticipant.Id)
        {
            return;
        }

        isDownloading = false;

        uiContext.Post(_ =>
        {
            UpdateIconOutOfState();
            fileNameAndSize.Text = $"Ошибка: {eventMessage.ErrorMessage}";
            splitContainerDownload.Panel2Collapsed = true;
        }, this);

        fileTransferProgressSubsciptionToken.Dispose();

        await Task.CompletedTask;
    }

    private async Task OnFileTransferCompleted(FileTransferCompletedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.FileMetadataId != fileMetadataMessage.Id)
        {
            return;
        }

        downloaded = true;
        fileMetadataMessage.FilePath = eventMessage.FilePath;
        fileTransferProgressSubsciptionToken?.Dispose();

        isDownloading = false;

        uiContext.Post(_ =>
        {
            UpdateIconOutOfState();
            splitContainerDownload.Panel2Collapsed = true;
            fileNameAndSize.Text = string.Empty;
            fileNameAndSize.AutoSize = false;
            fileNameAndSize.BackColor = Color.Transparent;
            fileNameAndSize.Dock = DockStyle.Fill;
            AskParentForResize?.Invoke();
        }, this);

        await Task.CompletedTask;
    }

    ///<inheritdoc />
    public override void ApplyStylings()
    {
        if (removeHeaders)
        {
            Height -= Convert.ToInt32(TableLayoutPanel.RowStyles[0].Height);
        }
        base.ApplyStylings();

        fileNameAndSize.BackColor = ColorScheme.PrimaryAccent;
        fileNameAndSize.ForeColor = ColorScheme.TextOnAccent;
        fileNameAndSize.Font = FontScheme.Heading3;
    }

    private void FillLabels()
    {
        if (downloaded)
        {
            fileNameAndSize.AutoSize = false;
            fileNameAndSize.BackColor = Color.Transparent;
            fileNameAndSize.Dock = DockStyle.Fill;
        }
        fileNameAndSize.Text = downloaded
            ? string.Empty
            : $"{fileMetadataMessage.FileName} {fileTransferFeatureCollection.FileHelperService
            .FormatFileSize(fileMetadataMessage.FileSize)}";
    }

    private void UpdateIconOutOfState()
    {
        if (isDownloading)
        {
            tableLayoutPanelImage.BackgroundImage = Resources.close;
        }
        else
        {
            if (downloaded)
            {
                tableLayoutPanelImage.BackColor = ColorScheme.PrimaryAccent;
                tableLayoutPanelImage.BackgroundImage = null;
            }
            else
            {
                tableLayoutPanelImage.BackgroundImage = Resources.download;
            }
        }
    }

    /// <inheritdoc />
    public int ResizeOutOfPrefferedSize()
    {
        if (!downloaded || string.IsNullOrEmpty(fileMetadataMessage.FilePath))
        {
            return Height;
        }

        var parentWidth = Convert.ToInt32(Parent!.Width * 0.85);

        if (cachedImgSize == null)
        {
            cachedImgSize = ImageHelper.GetDimensions(fileMetadataMessage.FilePath);
        }

        var imgSize = cachedImgSize.Value;
        var ratio = (double)imgSize.Width / imgSize.Height;
        var wantedWidth = Math.Min(parentWidth, imgSize.Width);

        if (Width == wantedWidth)
        {
            return Height;
        }

        Width = wantedWidth;

        var wantedHeight = (int)(wantedWidth / ratio);
        var headerHeight = removeHeaders ? 0
            : Convert.ToInt32(TableLayoutPanel.RowStyles[0].Height);
        var resultHeight = headerHeight + wantedHeight;

        fileNameAndSize.Image = cachedFileFormat == ".gif"
            ? Image.FromFile(fileMetadataMessage.FilePath)
            : ImageHelper.LoadScaled(fileMetadataMessage.FilePath, wantedWidth);

        Height = resultHeight;
        return resultHeight;
    }

    private void fileNameAndSize_Click(object sender, EventArgs e)
    {
        if (isDownloading)
        {
            OnCancelRequested?.Invoke();
            isDownloading = false;
            return;
        }

        if (!downloaded)
        {
            OnDownloadRequested?.Invoke();
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    void IDisposable.Dispose()
    {
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
    }
}
