using MIN.Core.Events.Contracts;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Properties;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения, представляющая файл от пользователя
/// </summary>
public partial class ChatFileMessageCard : UserControl, IDisposable
{
    private readonly IFileTransferFeatureCollection fileTransferFeatureCollection;
    private readonly IEventBus eventBus;
    private readonly FileMetadataMessage fileMetadataMessage;
    private readonly SynchronizationContext uiContext;
    private readonly bool isLocal;
    private readonly bool removeHeaders;
    private readonly bool hostMessage;
    private HashSet<IDisposable> eventTokens = null!;
    private bool isDownloading;
    private bool downloaded;
    private string savedFileType = string.Empty;
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
    /// Инициализирует новый экземпляр <see cref="ChatFileMessageCard"/>
    /// </summary>
    public ChatFileMessageCard(IFileTransferFeatureCollection fileTransferFeatureCollection,
        IEventBus eventBus,
        FileMetadataMessage fileMetadataMessage,
        bool isLocal,
        bool hostMessage,
        bool removeHeaders)
    {
        InitializeComponent();
        this.fileTransferFeatureCollection = fileTransferFeatureCollection;
        this.eventBus = eventBus;
        this.fileMetadataMessage = fileMetadataMessage;
        this.removeHeaders = removeHeaders;
        this.hostMessage = hostMessage;
        this.isLocal = isLocal;

        downloaded = fileMetadataMessage.FilePath != null;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("");

        FillLabels();
        ApplyStylings();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        eventTokens = [
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

            var progress = 100 * (double)e.BytesReceived / fileMetadataMessage.FileSize;
            uiContext.Post(_ =>
            {
                downloadProgressBar.Value = (int)Math.Min(progress, 100);
                fileSize.Text = $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(e.BytesReceived)}" +
                    $" / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
            }, this);

            return;
        });

        uiContext.Post(_ =>
        {
            fileSize.Text = $"0 / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
            splitContainerDownload.Panel2Collapsed = false;
        }, this);

        await Task.CompletedTask;
    }

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != fileMetadataMessage.RoomId || eventMessage.FileMetadataId != fileMetadataMessage.Id)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            fileSize.Text = $"Ошибка: {eventMessage.ErrorMessage}";
            splitContainerDownload.Panel2Collapsed = true;
        }, this);

        fileTransferProgressSubsciptionToken.Dispose();

        isDownloading = false;

        await Task.CompletedTask;
    }


    private async Task OnFileTransferCompleted(FileTransferCompletedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.RoomId != fileMetadataMessage.RoomId || eventMessage.FileMetadataId != fileMetadataMessage.Id)
        {
            return;
        }

        uiContext.Post(_ =>
        {
            splitContainerDownload.Panel2Collapsed = true;
        }, this);


        downloaded = true;
        fileMetadataMessage.FilePath = eventMessage.FilePath;
        fileTransferProgressSubsciptionToken?.Dispose();

        isDownloading = false;

        await Task.CompletedTask;
    }

    ///<inheritdoc />
    public void ApplyStylings()
    {
        if (removeHeaders)
        {
            Height -= Convert.ToInt32(tableLayoutPanel.RowStyles[0].Height);
            tableLayoutPanel.RowStyles[0].Height = 0;
            senderName.Visible = false;
            sendRole.Visible = false;
        }

        var senderColor = isLocal
            ? ColorScheme.OutgoingMessageBackground
            : ColorScheme.IncomingMessageBackground;

        senderName.BackColor = senderColor;
        sendRole.BackColor = senderColor;
        sendTime.BackColor = senderColor;
        tableLayoutPanelLabels.BackColor = senderColor;
        tableLayoutPanel.BackColor = senderColor;
        fileInterractButton.BackColor = ColorScheme.SecondaryAccent;
        fileInterractButton.ForeColor = ColorScheme.TextOnAccent;

        fileName.Font = FontScheme.Default;
        fileSize.Font = FontScheme.Caption;
        fileInterractButton.Font = FontScheme.Monospace;
        senderName.Font = FontScheme.Monospace;
        sendRole.Font = FontScheme.MicroCaption;
        sendTime.Font = FontScheme.MicroCaption;
    }

    private void FillLabels()
    {
        senderName.Text = fileMetadataMessage.Sender.Name;
        sendRole.Text = hostMessage ? "Хост" : string.Empty;
        sendTime.Text = fileMetadataMessage.Timestamp.ToShortTimeString();
        fileName.Text = fileMetadataMessage.FileName;
        fileSize.Text = fileTransferFeatureCollection.FileHelperService
            .FormatFileSize(fileMetadataMessage.FileSize);
        fileInterractButton.Text = fileTransferFeatureCollection.FileHelperService
            .GetFileType(fileMetadataMessage.FileName)
            .Substring(1);
    }

    private void fileInterractButton_MouseHover(object sender, EventArgs e)
    {
        savedFileType = fileInterractButton.Text;
        fileInterractButton.Text = string.Empty;
        fileInterractButton.BackgroundImage = isDownloading
            ? Resources.compass
            : downloaded ? Resources.file : Resources.download;
    }

    private void fileInterractButton_MouseLeave(object sender, EventArgs e)
    {
        fileInterractButton.BackgroundImage = null;
        fileInterractButton.Text = savedFileType;
    }

    private void fileInterractButton_Click(object sender, EventArgs e)
    {
        if (isDownloading)
        {
            OnCancelRequested?.Invoke();
            isDownloading = false;
            return;
        }

        if (downloaded)
        {
            if (!Path.Exists(fileMetadataMessage.FilePath))
            {
                MessageBox.Show("Файл не нашёлся");
                downloaded = false;
                return;
            }
            MessageBox.Show("Типо открылся файл");
        }
        else
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
