using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models.Enums;
using MIN.Desktop.Infrastructure.Services;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;

namespace MIN.Desktop.ViewModels.Cards.Messages.Files;

public abstract partial class ChatFileBaseMessageViewModel : BaseChatMessageViewModel
{
    /// <summary>
    /// Функциональность для обмена файлов
    /// </summary>
    readonly protected IFileTransferFeatureCollection fileTransferFeatureCollection;

    /// <summary>
    /// Буфер обмена
    /// </summary>
    readonly protected IClipboard? clipboard;

    /// <summary>
    /// Scope событий для комнаты
    /// </summary>
    readonly protected IEventScope roomScope;

    /// <summary>
    /// Локальный пользователь
    /// </summary>
    readonly protected ParticipantInfo localParticipant;

    /// <summary>
    /// Закешированный формат
    /// </summary>
    readonly protected string cachedFormat;
    /// <summary>
    /// Файл загружен
    /// </summary>
    protected bool downloaded;

    /// <summary>
    /// Подкиска на прогресс скачивание
    /// </summary>
    protected IDisposable fileTransferProgressSubsciptionToken = null!;

    /// <summary>
    /// Сообщение файла
    /// </summary>
    public FileMetadataMessage FileMetadataMessage { get; init; }

    [ObservableProperty]
    public partial int DownloadProgress { get; set; }

    [ObservableProperty]
    public partial bool IsDownloading { get; set; }

    [ObservableProperty]
    public partial FileDownloadState FileDownloadState { get; set; } = FileDownloadState.None;

    /// <summary>
    /// Запрос на скавивания
    /// </summary>
    public event Func<Task>? OnDownloadRequested;

    /// <summary>
    /// Запрос отмены
    /// </summary>
    public event Func<Task>? OnCancelRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileBaseMessageViewModel"/>
    /// </summary>
    protected ChatFileBaseMessageViewModel(IFileTransferFeatureCollection fileTransferFeatureCollection,
        IEventScope roomScope,
        FileMetadataMessage fileMetadataMessage,
        Thickness timePadding,
        ParticipantInfo localParticipant,
        bool isHostMessage,
        bool removeHeaders,
        IClipboard? clipboard)
        : base(fileMetadataMessage.Id,
            fileMetadataMessage.Sender.Name,
            fileMetadataMessage.Timestamp,
            timePadding,
            localParticipant.Id == fileMetadataMessage.Sender.Id,
            isHostMessage,
            removeHeaders,
            fileMetadataMessage.RecipientId != null)
    {
        this.fileTransferFeatureCollection = fileTransferFeatureCollection;
        this.localParticipant = localParticipant;
        this.clipboard = clipboard;
        this.roomScope = roomScope;
        FileMetadataMessage = fileMetadataMessage;

        cachedFormat = fileTransferFeatureCollection.FileHelperService
            .GetFileType(fileMetadataMessage.FileName);

        downloaded = !string.IsNullOrEmpty(fileMetadataMessage.FilePath) || fileMetadataMessage.AsDownloaded;

        FillLabels();
        SubscribeToEvents(roomScope);
    }

    /// <summary>
    /// Заолнить поля
    /// </summary>
    protected virtual void FillLabels() { }

    /// <summary>
    /// Получен пакет файла
    /// </summary>
    protected virtual void OnTransferProgressUpdated(string formattedReceived, string formattedTotal) { }

    /// <summary>
    /// Передача прервалась
    /// </summary>
    protected virtual void OnTransferFailed(string errorMessage) { }

    /// <summary>
    /// Передача завершилась
    /// </summary>
    protected virtual void OnTransferCompleted() { }

    /// <summary>
    /// Вызвать событие скачивания
    /// </summary>
    protected void InvokeDownloadRequested()
    {
        OnDownloadRequested?.Invoke();
    }

    /// <summary>
    /// Вызвать событие отмены
    /// </summary>
    protected void InvokeCancelRequested()
    {
        OnCancelRequested?.Invoke();
    }

    private void SubscribeToEvents(IEventScope roomScope)
    {
        roomScope.Subscribe<FileTransferStartedEvent>(OnFileTransferStarted);
        roomScope.Subscribe<FileTransferFailedEvent>(OnFileTransferFailed);
        roomScope.Subscribe<FileTransferCompletedEvent>(OnFileTransferCompleted);
    }

    private async Task OnFileTransferStarted(FileTransferStartedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.FileMetadataId != FileMetadataMessage.Id)
        {
            return;
        }

        IsDownloading = true;

        fileTransferProgressSubsciptionToken = roomScope.Subscribe((FileTransferProgressEvent e, CancellationToken _) =>
        {
            if (eventMessage.FileMetadataId != FileMetadataMessage.Id)
            {
                return Task.CompletedTask;
            }

            var progress = 100 * e.BytesReceived / FileMetadataMessage.FileSize;
            DownloadProgress = (int)Math.Min(progress, 100);
            OnTransferProgressUpdated(
                fileTransferFeatureCollection.FileHelperService.FormatFileSize(e.BytesReceived),
                fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize));

            return Task.CompletedTask;
        });

        UpdateIconOutOfState();
        OnTransferProgressUpdated("0",
            fileTransferFeatureCollection.FileHelperService.FormatFileSize(eventMessage.FileSize));
    }

    private async Task OnFileTransferFailed(FileTransferFailedEvent eventMessage, CancellationToken cancellationToken)
    {
        if (eventMessage.FileMetadataId != FileMetadataMessage.Id
            || eventMessage.SenderId != localParticipant.Id || IsHost)
        {
            return;
        }

        IsDownloading = false;

        UpdateIconOutOfState();
        OnTransferFailed(eventMessage.ErrorMessage ?? string.Empty);

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
        OnTransferCompleted();
    }

    /// <summary>
    /// Обновить иконку состояния файла
    /// </summary>
    protected void UpdateIconOutOfState()
    {
        FileDownloadState = IsDownloading
            ? FileDownloadState.IsDownloading : downloaded
            ? FileDownloadState.Downloaded : FileDownloadState.NotDownloaded;
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

        var dir = Path.GetDirectoryName(FileMetadataMessage.FilePath);
        if (string.IsNullOrEmpty(dir))
        {
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start("explorer.exe", $"/select,\"{FileMetadataMessage.FilePath}\"");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", $"-R \"{FileMetadataMessage.FilePath}\"");
        }
        else // unix
        {
            Process.Start("xdg-open", dir);
        }
    }
}
