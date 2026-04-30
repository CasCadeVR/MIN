using MIN.FileTransfer.Services.Contracts.Interfaces;

namespace MIN.FileTransfer.DI.FeatureCollection;

/// <inheritdoc cref="IFileTransferFeatureCollection"/>
public class FileTransferFeatureCollection : IFileTransferFeatureCollection
{
    /// <inheritdoc />
    public IFileTransferService FileTransferService { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FileTransferFeatureCollection"/>
    /// </summary>
    public FileTransferFeatureCollection(IFileTransferService fileTransferService)
    {
        FileTransferService = fileTransferService;
    }
}
