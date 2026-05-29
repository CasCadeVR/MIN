using System.Diagnostics;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Desktop.Components.Controls.ContextMenuStrips;
using MIN.Desktop.Contracts.Schemes;
using MIN.Desktop.Properties;
using MIN.Desktop.Views.Components.ChatMessages;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.Components;

/// <summary>
/// Карточка сообщения, представляющая файл от пользователя
/// </summary>
public partial class ChatFileMessageCard : BaseChatMessageCard, IDisposable
{
    private readonly IFileTransferFeatureCollection fileTransferFeatureCollection = null!;
    private readonly IEventBus eventBus = null!;
    private readonly FileMetadataMessage fileMetadataMessage = null!;
    private readonly SynchronizationContext uiContext = null!;
    private readonly ParticipantInfo localParticipant = null!;
    private readonly string cachedFormat = string.Empty;
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

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileMessageCard"/>
    /// </summary>
    public ChatFileMessageCard() : base()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileMessageCard"/>
    /// </summary>
    public ChatFileMessageCard(IFileTransferFeatureCollection fileTransferFeatureCollection,
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

        cachedFormat = fileInterractButton.Text = fileTransferFeatureCollection.FileHelperService
            .GetFileType(fileMetadataMessage.FileName)
            .Substring(1);

        downloaded = !string.IsNullOrEmpty(fileMetadataMessage.FilePath) || fileMetadataMessage.AsDownloaded;

        uiContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("Must be created on UI thread");

        FillLabels();
        ApplyStylings();
        PerformLayout();
        InitializeContextMenu();

        SubscribeToEvents();
    }

    private void InitializeContextMenu()
    {
        var pictureBoxContextMenuStrip = new FileMessageContextMenuStrip();
        pictureBoxContextMenuStrip.OnItemClick += () => OnCardContextMenuStripClicked?.Invoke();
        pictureBoxContextMenuStrip.Items[0].Text = "Показать в проводнике";
        ContextMenuStrip = pictureBoxContextMenuStrip;
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

        fileTransferProgressSubsciptionToken = eventBus.Subscribe((FileTransferProgressEvent e, CancellationToken _) =>
        {
            if (eventMessage.RoomId != fileMetadataMessage.RoomId || eventMessage.FileMetadataId != fileMetadataMessage.Id)
            {
                return Task.CompletedTask;
            }

            var progress = 100 * e.BytesReceived / fileMetadataMessage.FileSize;
            uiContext.Post(_ =>
            {
                downloadProgressBar.Value = (int)Math.Min(progress, 100);
                fileSize.Text = $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(e.BytesReceived)}" +
                    $" / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
            }, this);

            return Task.CompletedTask;
        });

        uiContext.Post(_ =>
        {
            UpdateIconOutOfState();
            fileSize.Text = $"0 / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
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
            fileSize.Text = $"Ошибка: {eventMessage.ErrorMessage}";
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
            UpdateIconOutOfState();
            splitContainerDownload.Panel2Collapsed = true;
            fileSize.Text = fileTransferFeatureCollection.FileHelperService
                .FormatFileSize(fileMetadataMessage.FileSize);
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

        tableLayoutPanelLabels.BackColor = SenderColor;
        fileInterractButton.BackColor = ColorScheme.SecondaryAccent;
        fileInterractButton.ForeColor = ColorScheme.TextOnAccent;

        fileName.Font = FontScheme.Default;
        fileSize.Font = FontScheme.Caption;
        fileInterractButton.Font = FontScheme.Monospace;
    }

    private void FillLabels()
    {
        fileName.Text = fileMetadataMessage.FileName;
        fileSize.Text = fileTransferFeatureCollection.FileHelperService
            .FormatFileSize(fileMetadataMessage.FileSize);
        fileInterractButton.Text = fileTransferFeatureCollection.FileHelperService
            .GetFileType(fileMetadataMessage.FileName)
            .Substring(1);
    }

    private void fileInterractButton_MouseEnter(object sender, EventArgs e)
    {
        UpdateIconOutOfState();
    }

    private void UpdateIconOutOfState()
    {
        fileInterractButton.Text = string.Empty;
        fileInterractButton.BackgroundImage = isDownloading
            ? Resources.close : downloaded
            ? Resources.file : Resources.download;
    }

    private void fileInterractButton_MouseLeave(object sender, EventArgs e)
    {
        fileInterractButton.BackgroundImage = null;
        fileInterractButton.Text = cachedFormat;
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
            var path = fileMetadataMessage.FilePath;

            if (!Path.Exists(path))
            {
                MessageBox.Show("Файл не нашёлся");
                downloaded = false;
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
            }
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
