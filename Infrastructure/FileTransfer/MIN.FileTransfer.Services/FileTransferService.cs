using System.Collections.Concurrent;
using MIN.Core.Events.Contracts;
using MIN.Core.Stores.Contracts.Interfaces;
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
    private readonly string baseDirectory;
    private readonly IRoomStore roomStore;
    private readonly IEventBus eventBus;
    private readonly IChunkBufferAssembler chunkBufferAssembler;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, TransferInfo> activeTransfers = new();
    private readonly ConcurrentDictionary<Guid, string> pendingMetadata = new();
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FileTransferService"/>
    /// </summary>
    public FileTransferService(IRoomStore roomStore,
        IEventBus eventBus,
        IChunkBufferAssembler chunkBufferAssembler,
        ILoggerProvider logger)
    {
        this.roomStore = roomStore;
        this.eventBus = eventBus;
        this.chunkBufferAssembler = chunkBufferAssembler;
        this.logger = logger;
        baseDirectory = Path.Combine(AppContext.BaseDirectory, "RoomFiles");

        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }

        chunkBufferAssembler.MessageAssembled += OnMessageAssembled;
        chunkBufferAssembler.ChunkReceived += OnChunkReceived;
    }

    /// <inheritdoc />
    public string GetRoomDirectory(Guid roomId)
    {
        var room = roomStore.GetRoom(roomId);
        var roomDir = Path.Combine(baseDirectory, $"Файлы комнаты {room.Name}");
        if (!Directory.Exists(roomDir))
        {
            Directory.CreateDirectory(roomDir);
        }
        return roomDir;
    }

    /// <inheritdoc />
    public string? GetFilePath(Guid roomId, string fileName)
    {
        var filePath = Path.Combine(GetRoomDirectory(roomId), fileName);
        return File.Exists(filePath) ? filePath : null;
    }

    /// <inheritdoc />
    public async Task<string> SaveFileAsync(Guid roomId, string fileName, Stream fileStream, CancellationToken cancellationToken = default)
    {
        var roomDir = GetRoomDirectory(roomId);
        var filePath = Path.Combine(roomDir, fileName);

        await using var fileStreamOut = File.Create(filePath);
        await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

        return filePath;
    }

    Task<Stream?> IFileTransferService.OpenFileForReadingAsync(Guid roomId, string fileName, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(roomId, fileName);
        if (filePath == null)
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = File.OpenRead(filePath);
        return Task.FromResult<Stream?>(stream);
    }

    Task IFileTransferService.DeleteRoomFilesAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var roomDir = GetRoomDirectory(roomId);
        if (Directory.Exists(roomDir))
        {
            Directory.Delete(roomDir, recursive: true);
        }
        return Task.CompletedTask;
    }

    Task IFileTransferService.DeleteFileAsync(Guid roomId, string fileName, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(roomId, fileName);
        if (filePath != null)
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
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

    private async void OnMessageAssembled(object? sender, MessageAssembledEventArgs e)
    {
        try
        {
            if (!TryGetTransferInfo(e.StreamId, out var info))
            {
                return;
            }

            await using var ms = new MemoryStream(e.Data);
            var filePath = await SaveFileAsync(info.RoomId, info.FileName, ms);

            await eventBus.PublishAsync(new FileTransferCompletedEvent
            {
                RoomId = info.RoomId,
                TransferId = e.StreamId,
                FileName = info.FileName,
                FilePath = filePath,
            });

            RemoveTransfer(e.StreamId);
            RemovePendingMetadata(e.StreamId);
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при сохранении файла из потока {e.StreamId}: {ex.Message}");

            if (TryGetTransferInfo(e.StreamId, out var info))
            {
                await eventBus.PublishAsync(new FileTransferFailedEvent
                {
                    RoomId = info.RoomId,
                    TransferId = e.StreamId,
                    ErrorMessage = ex.Message,
                });

                RemoveTransfer(e.StreamId);
            }
        }
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
