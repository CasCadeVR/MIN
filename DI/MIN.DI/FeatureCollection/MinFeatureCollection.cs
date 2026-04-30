using MIN.Chat.DI.FeatureCollection;
using MIN.Core.DI.FeatureCollection;
using MIN.Discovery.DI.FeatureCollection;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.Helpers.DI.FeatureCollection;

namespace MIN.DI.FeatureCollection;

/// <inheritdoc cref="IMinFeatureCollection"/>
public class MinFeatureCollection : IMinFeatureCollection
{
    /// <inheritdoc />
    public IHelperFeatureCollection Helper { get; }

    /// <inheritdoc />
    public ICoreFeatureCollection Core { get; }

    /// <inheritdoc />
    public IChatFeatureCollection Chat { get; }

    /// <inheritdoc />
    public IFileTransferFeatureCollection FileTransfer { get; }

    /// <inheritdoc />
    public IDiscoveryFeatureCollection Discovery { get; }

    /// <inheritdoc />
    public Version Version { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MinFeatureCollection"/>
    /// </summary>
    public MinFeatureCollection(IHelperFeatureCollection helper,
        ICoreFeatureCollection core,
        IChatFeatureCollection chat,
        IFileTransferFeatureCollection fileTransfer,
        IDiscoveryFeatureCollection discovery,
        Version version)
    {
        Helper = helper;
        Core = core;
        Chat = chat;
        FileTransfer = fileTransfer;
        Discovery = discovery;
        Version = version;
    }
}
