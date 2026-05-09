using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Properties;
using MIN.Desktop.Views.Components.ChatMessages;
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

        downloaded = !string.IsNullOrEmpty(fileMetadataMessage.FilePath) || fileMetadataMessage.AsDownloaded;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("");

        FillLabels();
        ApplyStylings();
        UpdateIconOutOfState();
        PerformLayout();
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

    private async Task OnFileTransferStarted(FileTransferStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != fileMetadataMessage.RoomId || eventMessage.FileMetadataId != fileMetadataMessage.Id)
        {
            return;
        }

        isDownloading = true;

        fileTransferProgressSubsciptionToken = eventBus.Subscribe(async (FileTransferProgressEvent e, CancellationToken _) =>
        {
            if (eventMessage.RoomId != fileMetadataMessage.RoomId || eventMessage.FileMetadataId != fileMetadataMessage.Id)
            {
                return;
            }

            var progress = 100 * e.BytesReceived / fileMetadataMessage.FileSize;
            uiContext.Post(_ =>
            {
                downloadProgressBar.Value = (int)Math.Min(progress, 100);
                fileNameAndSize.Text = $"{fileMetadataMessage.FileName} " +
                    $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(e.BytesReceived)}" +
                    $" / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
            }, this);

            return;
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
        if (eventMessage.RoomId != fileMetadataMessage.RoomId
            || eventMessage.FileMetadataId != fileMetadataMessage.Id
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
        if (eventMessage.RoomId != fileMetadataMessage.RoomId || eventMessage.FileMetadataId != fileMetadataMessage.Id)
        {
            return;
        }

        downloaded = true;
        fileMetadataMessage.FilePath = eventMessage.FilePath;
        fileTransferProgressSubsciptionToken?.Dispose();

        isDownloading = false;

        uiContext.Post(_ =>
        {
            splitContainerDownload.Panel2Collapsed = true;
            fileNameAndSize.Text = string.Empty;
            fileNameAndSize.BackgroundImage = null;
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

        fileNameAndSize.BackColor = Color.Black;
        fileNameAndSize.Font = FontScheme.Heading3;
        fileNameAndSize.BackgroundImageLayout = ImageLayout.Zoom;
    }

    private void FillLabels()
    {
        fileNameAndSize.Text = downloaded
            ? string.Empty
            : $"{fileMetadataMessage.FileName} {fileTransferFeatureCollection.FileHelperService
            .FormatFileSize(fileMetadataMessage.FileSize)}";
    }

    private void UpdateIconOutOfState()
    {
        if (isDownloading)
        {
            fileNameAndSize.BackgroundImage = Resources.close;
        }
        else if (!downloaded)
        {
            fileNameAndSize.BackgroundImage = Resources.download;
        }
    }

    /// <summary>
    /// Подстроивает размеры сообщений под содержимое внутри и возвращает полученную высоту
    /// </summary>
    /// <returns>
    /// Вычисленную высоту, исходя из содержимого
    /// </returns>
    public int ResizeOutOfPrefferedSize()
    {
        if (!downloaded || string.IsNullOrEmpty(fileMetadataMessage.FilePath))
        {
            return Height;
        }

        var wantedWidth = Convert.ToInt32(Parent!.Width * 0.85);

        var imgSize = ImageHelper.GetDimensions(fileMetadataMessage.FilePath);
        var ratio = (double)imgSize.Width / imgSize.Height;
        var width = Math.Min(wantedWidth, imgSize.Width);
        var height = (int)(width / ratio);

        Width = width;

        var headerHeight = removeHeaders ? 0
            : Convert.ToInt32(TableLayoutPanel.RowStyles[0].Height);
        Height = headerHeight + height;

        fileNameAndSize.Image = ImageHelper.LoadScaled(fileMetadataMessage.FilePath, width);

        return Height;
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
