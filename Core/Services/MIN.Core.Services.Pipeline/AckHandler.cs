using MIN.Core.Headers.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Pipeline;
using MIN.Core.Streaming.Contracts.Interfaces;

namespace MIN.Core.Services.Pipeline;

/// <summary>
/// Обработчик подтверждений получения пакетов потоков (ACK)
/// </summary>
public sealed class AckHandler : IAckHandler
{
    private readonly IHeaderManager headerManager;
    private readonly IStreamManager streamManager;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AckHandler"/>
    /// </summary>
    public AckHandler(IHeaderManager headerManager, IStreamManager streamManager)
    {
        this.headerManager = headerManager;
        this.streamManager = streamManager;
    }

    bool IAckHandler.CanHandle(byte[] data) => headerManager.IsAck(data);

    void IAckHandler.Handle(byte[] data) => streamManager.ProcessAck(data);
}
