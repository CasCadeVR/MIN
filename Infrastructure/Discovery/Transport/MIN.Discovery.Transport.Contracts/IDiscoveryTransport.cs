using MIN.Discovery.Transport.Contracts.Events;

namespace MIN.Discovery.Transport.Contracts;

/// <summary>
/// Транспортный уровень для широковещательного обнаружения комнат
/// </summary>
public interface IDiscoveryTransport
{
    /// <summary>
    /// Событие получения сырых данных от транспорта
    /// </summary>
    event EventHandler<DiscoveryRawMessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Отправить данные
    /// </summary>
    Task BroadcastAsync(byte[] data, TimeSpan timeout, CancellationToken ct = default);

    /// <summary>
    /// Начать прослушивание широковещательных сообщений, с отправкой ответа
    /// </summary>
    Task StartListeningAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить прослушивание
    /// </summary>
    Task StopListeningAsync();
}
