using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using Avalonia.Labs.Gif;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Desktop.Contracts.Models.Enums;
using MIN.Desktop.Infrastructure.Services;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.ViewModels.Cards.Messages.Files;

/// <summary>
/// Сообщение приложенного файла в виде превью фото
/// </summary>
public partial class ChatFileImagePreviewMessageViewModel : BaseChatMessageViewModel
{
    private readonly IFileTransferFeatureCollection fileTransferFeatureCollection = null!;
    private readonly IEventBus eventBus = null!;
    private readonly IClipboard? clipboard;
    private readonly ParticipantInfo localParticipant = null!;
    private readonly string cachedFormat = string.Empty;

    private HashSet<IDisposable> eventTokens = null!;
    private bool downloaded;
    private IDisposable fileTransferProgressSubsciptionToken = null!;

    // GIF

    private FileStream? gifStream;
    private GifStreamSource gifPreviewImage = null!;

    /// <summary>
    /// .gif Превью изображения
    /// </summary>
    public GifStreamSource GifPreviewImage
    {
        get => gifPreviewImage;
        private set
        {
            gifPreviewImage = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Содержимое сообщения файла
    /// </summary>
    public FileMetadataMessage FileMetadataMessage { get; init; }

    /// <summary>
    /// Прогресс скачивания
    /// </summary>
    [ObservableProperty]
    public partial int DownloadProgress { get; set; }

    /// <summary>
    /// Идёт ли сейчас скачивание
    /// </summary>
    [ObservableProperty]
    public partial bool IsDownloading { get; set; }

    /// <summary>
    /// Имя и размер фото
    /// </summary>
    [ObservableProperty]
    public partial string FileNameAndSize { get; set; } = string.Empty;

    /// <summary>
    /// Превью изображения
    /// </summary>
    [ObservableProperty]
    public partial IImage? PreviewImage { get; set; }

    /// <summary>
    /// Состояния скачивания файла
    /// </summary>
    [ObservableProperty]
    public partial FileDownloadState FileDownloadState { get; set; } = FileDownloadState.None;

    /// <summary>
    /// Событие, возникающее по нажатию на кнопку скачать
    /// </summary>
    public event Func<Task>? OnDownloadRequested;

    /// <summary>
    /// Событие, возникающее по нажатию на кнопку отмены загрузки
    /// </summary>
    public event Func<Task>? OnCancelRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileImagePreviewMessageViewModel"/>
    /// </summary>
    public ChatFileImagePreviewMessageViewModel(IFileTransferFeatureCollection fileTransferFeatureCollection,
        IEventBus eventBus,
        FileMetadataMessage fileMetadataMessage,
        Thickness timePadding,
        ParticipantInfo localParticipant,
        bool isHostMessage,
        bool removeHeaders,
        IClipboard? clipboard)
        : base(fileMetadataMessage.Sender.Name,
            fileMetadataMessage.Timestamp,
            timePadding,
            localParticipant.Id == fileMetadataMessage.Sender.Id,
            isHostMessage,
            removeHeaders,
            fileMetadataMessage.RecipientId != null)
    {
        this.fileTransferFeatureCollection = fileTransferFeatureCollection;
        this.eventBus = eventBus;
        this.localParticipant = localParticipant;
        this.clipboard = clipboard;
        FileMetadataMessage = fileMetadataMessage;

        cachedFormat = fileTransferFeatureCollection.FileHelperService
            .GetFileType(fileMetadataMessage.FileName);

        downloaded = !string.IsNullOrEmpty(fileMetadataMessage.FilePath) || fileMetadataMessage.AsDownloaded;

        FillLabels();
        SubscribeToEvents();
    }
    private void FillLabels()
    {
        FileNameAndSize = downloaded
            ? string.Empty
            : $"{FileMetadataMessage.FileName} {fileTransferFeatureCollection.FileHelperService
            .FormatFileSize(FileMetadataMessage.FileSize)}";

        LoadImage(FileMetadataMessage.FilePath ?? string.Empty);
        UpdateIconOutOfState();
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
        if (eventMessage.FileMetadataId != FileMetadataMessage.Id)
        {
            return;
        }

        IsDownloading = true;

        fileTransferProgressSubsciptionToken = eventBus.Subscribe((FileTransferProgressEvent e, CancellationToken _) =>
        {
            if (eventMessage.FileMetadataId != FileMetadataMessage.Id)
            {
                return Task.CompletedTask;
            }

            var progress = 100 * e.BytesReceived / FileMetadataMessage.FileSize;
            DownloadProgress = (int)Math.Min(progress, 100);
            FileNameAndSize = $"{FileMetadataMessage.FileName} " +
                    $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(e.BytesReceived)}" +
                    $" / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";

            return Task.CompletedTask;
        });

        UpdateIconOutOfState();

        FileNameAndSize = $"{FileMetadataMessage.FileName} 0 / " +
                $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";
    }

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.FileMetadataId != FileMetadataMessage.Id
            || eventMessage.SenderId != localParticipant.Id)
        {
            return;
        }

        IsDownloading = false;

        UpdateIconOutOfState();
        FileNameAndSize = $"Ошибка: {eventMessage.ErrorMessage}";

        fileTransferProgressSubsciptionToken.Dispose();
    }

    private async Task OnFileTransferCompleted(FileTransferCompletedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.FileMetadataId != FileMetadataMessage.Id)
        {
            return;
        }

        downloaded = true;
        FileMetadataMessage.FilePath = eventMessage.FilePath;
        fileTransferProgressSubsciptionToken?.Dispose();

        IsDownloading = false;

        UpdateIconOutOfState();
        FileNameAndSize = string.Empty;
        LoadImage(FileMetadataMessage.FilePath);
    }

    private void UpdateIconOutOfState()
    {
        FileDownloadState = IsDownloading
            ? FileDownloadState.IsDownloading : downloaded
            ? FileDownloadState.Downloaded : FileDownloadState.NotDownloaded;
    }

    private void LoadImage(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        gifStream?.Dispose();
        gifStream = null;

        if (cachedFormat == "gif")
        {
            gifStream = File.OpenRead(filePath);
            GifPreviewImage = GifStreamSource.FromStream(gifStream);
        }
        else
        {
            PreviewImage = new Bitmap(filePath);
        }
    }

    [RelayCommand]
    private void InteractionClick()
    {
        if (IsDownloading)
        {
            OnCancelRequested?.Invoke();
            IsDownloading = false;
            return;
        }

        if (!downloaded)
        {
            OnDownloadRequested?.Invoke();
        }
    }

    [RelayCommand]
    private async Task CopyNameToClipboard()
    {
        if (clipboard != null)
        {
            await clipboard.SetTextAsync(FileMetadataMessage.FileName);
            InAppNotifier.Info("Скопировано в буфер обмена");
        }
    }

    [RelayCommand]
    private async Task ShowInFolder()
    {
        if (!Path.Exists(FileMetadataMessage.FilePath))
        {
            InAppNotifier.Warning("Файл не нашёлся");
            return;
        }

        Process.Start("explorer.exe", $"/select,\"{FileMetadataMessage.FilePath}\"");
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public override void Dispose()
    {
        gifStream?.Dispose();
        gifStream = null;

        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
        base.Dispose();
    }
}
