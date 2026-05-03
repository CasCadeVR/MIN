using System.Collections.Concurrent;
using MIN.Core.Events.Contracts;
using MIN.Core.Streaming.Contracts.Events;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
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

    void IFileTransferService.RegisterTransfer(Guid transferId, Guid roomId, FileTransferDirection direction, string fileName)
    {
        activeTransfers[transferId] = new TransferInfo
        {
            TransferId = transferId,
            RoomId = roomId,
            Direction = direction,
            FileName = fileName,
        };
    }

    /// <inheritdoc />
    public bool TryGetTransferInfo(Guid transferId, out TransferInfo info)
        => activeTransfers.TryGetValue(transferId, out info!);

    /// <inheritdoc />
    public void RemoveTransfer(Guid transferId)
    {
        activeTransfers.TryRemove(transferId, out _);
    }

    void IFileTransferService.RegisterPendingMetadata(Guid transferId, string fileName)
    {
        pendingMetadata[transferId] = fileName;
    }

    bool IFileTransferService.TryGetPendingFileName(Guid transferId, out string fileName)
        => pendingMetadata.TryGetValue(transferId, out fileName!);

    /// <inheritdoc />
    public void RemovePendingMetadata(Guid transferId)
    {
        pendingMetadata.TryRemove(transferId, out _);
    }

    /// <inheritdoc />
    public async Task OnFileDataReceivedAsync(Guid transferId, byte[] data, CancellationToken cancellationToken = default)
    {
        if (!TryGetTransferInfo(transferId, out var info))
        {
            return;
        }

        try
        {
            await using var ms = new MemoryStream(data);
            var finalFileName = await fileStorageService.SaveFileAsync(info.RoomId, ms, info.FileName, cancellationToken);

            await eventBus.PublishAsync(new FileTransferCompletedEvent
            {
                RoomId = info.RoomId,
                TransferId = transferId,
                FileName = finalFileName,
                FilePath = fileStorageService.GetFilePath(info.RoomId, finalFileName) ?? string.Empty,
            });

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
                ErrorMessage = ex.Message,
            });

            RemoveTransfer(transferId);
        }
    }

    private async void OnMessageAssembled(object? sender, MessageAssembledEventArgs e)
    {
        await OnFileDataReceivedAsync(e.StreamId, e.Data);
    }

    private async void OnChunkReceived(object? sender, ChunkReceivedEventArgs e)
    {
        if (TryGetTransferInfo(e.StreamId, out var info))
        {
            await eventBus.PublishAsync(new FileTransferProgressEvent
            {
                RoomId = info.RoomId,
                TransferId = e.StreamId,
                BytesReceived = 0,
                TotalBytes = 0,
            });
        }
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
        activeTransfers.Clear();
        pendingMetadata.Clear();
    }
}
