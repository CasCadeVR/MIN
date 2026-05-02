using MIN.Core.Stores.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Services;

/// <inheritdoc cref="IFileStorageService"/>
public sealed class FileStorageService : IFileStorageService
{
    private readonly string baseDirectory;
    private readonly IRoomStore roomStore;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FileStorageService"/>
    /// </summary>
    public FileStorageService(IRoomStore roomStore)
    {
        this.roomStore = roomStore;
        baseDirectory = Path.Combine(AppContext.BaseDirectory, "RoomFiles");

        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }
    }

    /// <inheritdoc />
    public string GetRoomDirectory(Guid roomId)
    {
        var room = roomStore.GetRoom(roomId);
        var roomDir = Path.Combine(baseDirectory, $"Файлы комнаты {room.Name}");
        return roomDir;
    }

    /// <inheritdoc />
    public void EnsureRoomDirectoryExists(Guid roomId)
    {
        var roomDir = GetRoomDirectory(roomId);
        if (!Directory.Exists(roomDir))
        {
            Directory.CreateDirectory(roomDir);
        }
    }

    bool IFileStorageService.FileExists(Guid roomId, string fileName)
    {
        var filePath = Path.Combine(GetRoomDirectory(roomId), fileName);
        return File.Exists(filePath);
    }

    /// <inheritdoc />
    public string? GetFilePath(Guid roomId, string fileName)
    {
        var filePath = Path.Combine(GetRoomDirectory(roomId), fileName);
        return File.Exists(filePath) ? filePath : null;
    }

    async Task<string> IFileStorageService.SaveFileAsync(Guid roomId, Stream source, string fileName, CancellationToken cancellationToken)
    {
        EnsureRoomDirectoryExists(roomId);
        var roomDir = GetRoomDirectory(roomId);

        var finalFileName = ResolveUniqueFileName(roomDir, fileName);
        var filePath = Path.Combine(roomDir, finalFileName);

        await using var fileStream = File.Create(filePath);
        await source.CopyToAsync(fileStream, cancellationToken);

        return finalFileName;
    }

    Task<Stream?> IFileStorageService.OpenFileForReadingAsync(Guid roomId, string fileName, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(roomId, fileName);
        if (filePath == null)
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = File.OpenRead(filePath);
        return Task.FromResult<Stream?>(stream);
    }

    Task IFileStorageService.DeleteFileAsync(Guid roomId, string fileName, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(roomId, fileName);
        if (filePath != null)
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    Task IFileStorageService.DeleteRoomFilesAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var roomDir = GetRoomDirectory(roomId);
        if (Directory.Exists(roomDir))
        {
            Directory.Delete(roomDir, recursive: true);
        }
        return Task.CompletedTask;
    }

    private static string ResolveUniqueFileName(string directory, string fileName)
    {
        var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var candidate = fileName;
        var counter = 1;

        while (File.Exists(Path.Combine(directory, candidate)))
        {
            candidate = $"{nameWithoutExt}_{counter}{extension}";
            counter++;
        }

        return candidate;
    }
}
