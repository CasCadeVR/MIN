using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Desktop.Infrastructure.Services;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.ViewModels.Cards.Messages.Files;

/// <summary>
/// Сообщение приложенного файла
/// </summary>
public partial class ChatFileMessageViewModel : BaseChatMessageViewModel
{
    private readonly IFileTransferFeatureCollection fileTransferFeatureCollection = null!;
    private readonly IEventBus eventBus = null!;
    private readonly ParticipantInfo localParticipant = null!;
    private readonly string cachedFormat = string.Empty;

    private HashSet<IDisposable> eventTokens = null!;
    private bool downloaded;
    private IDisposable fileTransferProgressSubsciptionToken = null!;

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
    /// Универсальный статус взаимодействия с файлом
    /// </summary>
    [ObservableProperty]
    public partial string InteractionFileStatus { get; set; } = string.Empty;

    /// <summary>
    /// Универсальный статус файла (скачивание, размер и ошибка)
    /// </summary>
    [ObservableProperty]
    public partial string UniversalFileStatus { get; set; } = string.Empty;

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
    /// Инициализирует новый экземпляр <see cref="ChatFileMessageViewModel"/>
    /// </summary>
    public ChatFileMessageViewModel(IFileTransferFeatureCollection fileTransferFeatureCollection,
        IEventBus eventBus,
        FileMetadataMessage fileMetadataMessage,
        Thickness timePadding,
        ParticipantInfo localParticipant,
        bool isHostMessage,
        bool removeHeaders)
        : base(fileMetadataMessage.Sender.Name,
            fileMetadataMessage.Timestamp,
            timePadding,
            localParticipant.Id == fileMetadataMessage.Sender.Id,
            isHostMessage,
            removeHeaders)
    {
        this.fileTransferFeatureCollection = fileTransferFeatureCollection;
        this.eventBus = eventBus;
        this.localParticipant = localParticipant;
        FileMetadataMessage = fileMetadataMessage;

        cachedFormat = fileTransferFeatureCollection.FileHelperService
            .GetFileType(fileMetadataMessage.FileName);

        downloaded = !string.IsNullOrEmpty(fileMetadataMessage.FilePath) || fileMetadataMessage.AsDownloaded;

        FillLabels();
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
            UniversalFileStatus = $"{fileTransferFeatureCollection.FileHelperService.FormatFileSize(e.BytesReceived)}" +
                $" / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";

            return Task.CompletedTask;
        });

        UpdateIconOutOfState();
        UniversalFileStatus = $"0 / {fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize)}";

        await Task.CompletedTask;
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
        UniversalFileStatus = $"Ошибка: {eventMessage.ErrorMessage}";

        fileTransferProgressSubsciptionToken.Dispose();

        await Task.CompletedTask;
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
        UniversalFileStatus = fileTransferFeatureCollection.FileHelperService
            .FormatFileSize(FileMetadataMessage.FileSize);
    }

    private void FillLabels()
    {
        UniversalFileStatus = fileTransferFeatureCollection.FileHelperService
            .FormatFileSize(FileMetadataMessage.FileSize);
        InteractionFileStatus = cachedFormat;
    }

    private void fileInterractButton_MouseEnter(object sender, EventArgs e)
    {
        UpdateIconOutOfState();
    }

    private void UpdateIconOutOfState()
    {
        InteractionFileStatus = string.Empty;
        //fileInterractButton.BackgroundImage = isDownloading
        //    ? Resources.close : downloaded
        //    ? Resources.file : Resources.download;
    }

    private void fileInterractButton_MouseLeave(object sender, EventArgs e)
    {
        //fileInterractButton.BackgroundImage = null;
        InteractionFileStatus = cachedFormat;
    }

    private void fileInterractButton_Click(object sender, EventArgs e)
    {
        if (IsDownloading)
        {
            OnCancelRequested?.Invoke();
            IsDownloading = false;
            return;
        }

        if (downloaded)
        {
            var path = FileMetadataMessage.FilePath;

            if (!Path.Exists(path))
            {
                InAppNotifier.Warning("Файл не нашёлся");
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
                InAppNotifier.Error($"Не удалось открыть файл: {ex.Message}");
            }
        }
        else
        {
            OnDownloadRequested?.Invoke();
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public override void Dispose()
    {
        foreach (var token in eventTokens)
        {
            token.Dispose();
        }
        base.Dispose();
    }
}
