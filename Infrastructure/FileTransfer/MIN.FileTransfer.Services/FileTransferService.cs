using System.Collections.Concurrent;
using MIN.Core.Events.Contracts;
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
    public FileTransferService(
        IEventBus eventBus,
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
    }

    void IFileTransferService.RegisterTransfer(TransferInfo info)
    {
        logger.Log($"Регистрирую transfer: TransferId={info.TransferId}, Room={info.RoomId}, Direction={info.Direction}, File={info.FileName}");
        activeTransfers[info.TransferId] = info;
    }

    /// <inheritdoc />
    public bool TryGetTransferInfo(Guid transferId, out TransferInfo info)
        => activeTransfers.TryGetValue(transferId, out info!);

    /// <inheritdoc />
    public void RemoveTransfer(Guid transferId)
    {
        logger.Log($"Удаляю transfer {transferId}");
        if (activeTransfers.TryRemove(transferId, out var info))
        {
            chunkBufferAssembler.TryRemoveStream(transferId);
            info.CancellationTokenSource?.Cancel();
        }
    }

    void IFileTransferService.RegisterPendingMetadata(Guid transferId, string fileName)
    {
        logger.Log($"Регистрирую pending метаданные: TransferId={transferId}, File={fileName}");
        pendingMetadata[transferId] = fileName;
    }

    bool IFileTransferService.TryGetPendingFileName(Guid transferId, out string fileName)
        => pendingMetadata.TryGetValue(transferId, out fileName!);

    /// <inheritdoc />
    public void RemovePendingMetadata(Guid transferId)
    {
        pendingMetadata.TryRemove(transferId, out _);
    }

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
            RemovePendingMetadata(transferId);
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
            });

            RemoveTransfer(transferId);
        }
    }

    private async void OnMessageAssembled(object? sender, MessageAssembledEventArgs e)
    {
        logger.Log($"Сообщение собрано: StreamId={e.StreamId}, Size={e.Data.Length} байт");
        await OnFileDataReceivedAsync(e.StreamId, e.Data);
    }

    private async void OnChunkReceived(object? sender, ChunkReceivedEventArgs e)
    {
        await eventBus.PublishAsync(new FileTransferProgressEvent
        {
            RoomId = e.RoomId,
            TransferId = e.StreamId,
            BytesReceived = e.ReceivedBytes,
        });
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        logger.Log("FileTransferService disposed, очищаю активные transfer'ы");
        chunkBufferAssembler.MessageAssembled -= OnMessageAssembled;
        chunkBufferAssembler.ChunkReceived -= OnChunkReceived;
        activeTransfers.Clear();
        pendingMetadata.Clear();
        fileMetadataRegistry.Clear();
    }
}
