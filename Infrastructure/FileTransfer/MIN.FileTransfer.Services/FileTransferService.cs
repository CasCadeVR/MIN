using System.Collections.Concurrent;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Services;

/// <inheritdoc cref="IFileTransferService"/>
public sealed class FileTransferService : IFileTransferService, IDisposable
{
    private readonly IEventBus eventBus;
    private readonly IChunkBufferAssembler chunkBufferAssembler;
    private readonly IFileStorageService fileStorageService;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, TransferInfo> activeTransfers = new();
    private readonly ConcurrentDictionary<Guid, string> pendingMetadata = new();
    private readonly ConcurrentDictionary<Guid, FileMetadataInfo> fileMetadataRegistry = new();
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FileTransferService"/>
    /// </summary>
    public FileTransferService(IEventBus eventBus,
        IChunkBufferAssembler chunkBufferAssembler,
        IFileStorageService fileStorageService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.chunkBufferAssembler = chunkBufferAssembler;
        this.fileStorageService = fileStorageService;
        this.logger = logger;

        chunkBufferAssembler.MessageAssembled += OnMessageAssembled;
        chunkBufferAssembler.ChunkReceived += OnChunkReceived;
        chunkBufferAssembler.OnStreamFailed += OnStreamFailed;
    }

    void IFileTransferService.RegisterTransfer(TransferInfo info)
    {
        logger.Log($"Регистрирую transfer: TransferId={info.TransferId}, Room={info.RoomId}, Direction={info.Direction}, File={info.FileName}");
        activeTransfers[info.TransferId] = info;
    }

    /// <inheritdoc />
    public bool TryGetTransferInfo(Guid transferId, out TransferInfo info)
        => activeTransfers.TryGetValue(transferId, out info!);

    IEnumerable<TransferInfo> IFileTransferService.GetActiveTransfers()
        => activeTransfers.Values;

    /// <inheritdoc />
    public void RemoveTransfer(Guid transferId)
    {
        logger.Log($"Удаляю transfer {transferId}");
        if (activeTransfers.TryRemove(transferId, out var info))
        {
            chunkBufferAssembler.TryRemoveStream(transferId);
            info.CancellationTokenSource?.Cancel();
        }
        pendingMetadata.TryRemove(transferId, out _);
    }

    void IFileTransferService.RegisterPendingMetadata(Guid transferId, string fileName)
    {
        logger.Log($"Регистрирую pending метаданные: TransferId={transferId}, File={fileName}");
        pendingMetadata[transferId] = fileName;
    }

    bool IFileTransferService.TryGetPendingFileName(Guid transferId, out string fileName)
        => pendingMetadata.TryGetValue(transferId, out fileName!);

    void IFileTransferService.RegisterFileMetadata(Guid fileMetadataId, Guid roomId, string fileName, string? originalFilePath)
    {
        logger.Log($"Регистрирую метаданные файла: FileId={fileMetadataId}, Room={roomId}, File={fileName}, Path={originalFilePath ?? "RoomFiles"}");
        fileMetadataRegistry[fileMetadataId] = new FileMetadataInfo
        {
            FileMetadataId = fileMetadataId,
            RoomId = roomId,
            FileName = fileName,
            OriginalPath = originalFilePath,
            IsStoredOnServer = originalFilePath == null,
        };
    }

    bool IFileTransferService.TryGetFileMetadata(Guid fileMetadataId, out FileMetadataInfo info)
        => fileMetadataRegistry.TryGetValue(fileMetadataId, out info!);

    private async Task OnMessageAssembled(MessageAssembledEventArgs e, CancellationToken cancellationToken)
    {
        if (!e.IsRawPayload)
        {
            return;
        }

        if (e.FilePath == null)
        {
            await OnFileDataReceivedAsync(e.StreamId, e.Data ?? [], cancellationToken);
        }
        else
        {
            await OnRawFileReceivedAsync(e.StreamId, e.FilePath, cancellationToken);
        }

        logger.Log($"Сообщение собрано: StreamId={e.StreamId}, Size={e.Data?.Length ?? 0} байт");
    }

    /// <inheritdoc />
    public async Task OnFileDataReceivedAsync(Guid transferId, byte[] data, CancellationToken cancellationToken = default)
    {
        if (!TryGetTransferInfo(transferId, out var info))
        {
            logger.Log($"Получены данные для неизвестного transfer {transferId}, игнорирую");
            return;
        }

        logger.Log($"Сохраняю файл {info.FileName} ({data.Length} байт) для transfer {transferId}");

        try
        {
            await using var ms = new MemoryStream(data);
            var finalFileName = await fileStorageService.SaveFileAsync(info.RoomId, ms, info.FileName, cancellationToken);

            var filePath = fileStorageService.GetFilePath(info.RoomId, finalFileName) ?? string.Empty;
            logger.Log($"Файл сохранён: {finalFileName} → {filePath}");

            await eventBus.PublishAsync(new FileTransferCompletedEvent
            {
                RoomId = info.RoomId,
                TransferId = transferId,
                FileMetadataId = info.FileMetadataId,
                FileName = finalFileName,
                FilePath = filePath,
            }, cancellationToken);

            RemoveTransfer(transferId);

            await eventBus.PublishAsync(new FilePendingMetaDataReceivedEvent()
            {
                TransferId = transferId,
                FilePath = filePath,
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при сохранении файла из потока {transferId}: {ex.Message}");

            await eventBus.PublishAsync(new FileTransferFailedEvent
            {
                RoomId = info.RoomId,
                TransferId = transferId,
                SenderId = info.SenderId,
                ErrorMessage = ex.Message,
            }, cancellationToken);

            RemoveTransfer(transferId);
        }
    }

    private async Task OnRawFileReceivedAsync(Guid transferId, string tempFilePath, CancellationToken cancellationToken = default)
    {
        if (!TryGetTransferInfo(transferId, out var info))
        {
            logger.Log($"Получены данные для неизвестного transfer {transferId}, игнорирую");
            File.Delete(tempFilePath);
            return;
        }

        var fileSize = new FileInfo(tempFilePath).Length;
        logger.Log($"Перемещаю файл {info.FileName} ({fileSize} байт) из временного пути для transfer {transferId}");

        var movingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, info.CancellationTokenSource?.Token ?? default);

        try
        {
            var finalFileName = await fileStorageService.MoveTempFileToRoomAsync(info.RoomId, tempFilePath, info.FileName, movingCts.Token);
            var filePath = fileStorageService.GetFilePath(info.RoomId, finalFileName) ?? string.Empty;
            logger.Log($"Файл сохранён: {finalFileName} → {filePath}");

            await eventBus.PublishAsync(new FileTransferCompletedEvent
            {
                RoomId = info.RoomId,
                TransferId = transferId,
                FileMetadataId = info.FileMetadataId,
                FileName = finalFileName,
                FilePath = filePath,
            }, cancellationToken);

            RemoveTransfer(transferId);
            pendingMetadata.TryRemove(transferId, out _);

            await eventBus.PublishAsync(new FilePendingMetaDataReceivedEvent()
            {
                TransferId = transferId,
                FilePath = filePath,
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при перемещении файла из временного пути {transferId}: {ex.Message}");

            await eventBus.PublishAsync(new FileTransferFailedEvent
            {
                RoomId = info.RoomId,
                TransferId = transferId,
                SenderId = info.SenderId,
                ErrorMessage = ex.Message,
            }, cancellationToken);

            RemoveTransfer(transferId);
        }
    }

    private async Task OnStreamFailed(StreamFailedEventArgs e, CancellationToken cancellationToken)
    {
        logger.Log($"Ошибка при передачи файла из потока {e.StreamId}: {e.ErrorMessage}");

        if (TryGetTransferInfo(e.StreamId, out var info))
        {
            await eventBus.PublishAsync(new FileTransferFailedEvent
            {
                RoomId = info.RoomId,
                TransferId = info.TransferId,
                SenderId = info.SenderId,
                ErrorMessage = e.ErrorMessage,
            }, cancellationToken);

            RemoveTransfer(info.TransferId);
        }
    }

    private async Task OnChunkReceived(ChunkReceivedEventArgs e, CancellationToken cancellationToken)
    {
        if (!activeTransfers.TryGetValue(e.StreamId, out _))
        {
            return;
        }

        await eventBus.PublishAsync(new FileTransferProgressEvent
        {
            RoomId = e.RoomId,
            TransferId = e.StreamId,
            BytesReceived = e.ReceivedBytes,
        }, cancellationToken);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        chunkBufferAssembler.MessageAssembled -= OnMessageAssembled;
        chunkBufferAssembler.ChunkReceived -= OnChunkReceived;
        chunkBufferAssembler.OnStreamFailed -= OnStreamFailed;
        activeTransfers.Clear();
        pendingMetadata.Clear();
        fileMetadataRegistry.Clear();
    }
}
