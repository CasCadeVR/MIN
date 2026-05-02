using MIN.FileTransfer.Services.Contracts.Interfaces;

namespace MIN.FileTransfer.DI.FeatureCollection;

/// <inheritdoc cref="IFileTransferFeatureCollection"/>
public class FileTransferFeatureCollection : IFileTransferFeatureCollection
{
    /// <inheritdoc cref="IFileTransferService"/>
    public IFileTransferService FileTransferService { get; }

    /// <inheritdoc cref="IFileHelperService"/>
    public IFileHelperService FileHelperService { get; }

    /// <inheritdoc cref="IFileStorageService"/>
    public IFileStorageService FileStorageService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FileTransferFeatureCollection"/>
    /// </summary>
    public FileTransferFeatureCollection(
        IFileTransferService fileTransferService,
        IFileHelperService fileHelperService,
        IFileStorageService fileStorageService)
    {
        FileTransferService = fileTransferService;
        FileHelperService = fileHelperService;
        FileStorageService = fileStorageService;
    }
}
