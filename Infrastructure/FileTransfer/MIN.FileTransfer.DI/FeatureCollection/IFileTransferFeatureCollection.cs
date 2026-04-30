using MIN.FileTransfer.Services.Contracts.Interfaces;

namespace MIN.FileTransfer.DI.FeatureCollection;

/// <summary>
/// Набор функциональностей для FileTransfer
/// </summary>
public interface IFileTransferFeatureCollection
{
    /// <inheritdoc cref="IFileTransferService"/>
    IFileTransferService FileTransferService { get; }
}
